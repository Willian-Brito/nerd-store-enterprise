using FluentValidation.Results;
using MediatR;
using NSE.Core.Messages.Base;
using NSE.Core.Messages.Integration;
using NSE.Order.Application.DTOs;
using NSE.Order.Application.Events;
using NSE.Order.Domain.Entities.Vouchers.Specs;
using NSE.Order.Domain.Interfaces;
using NSE.Queue.Abstractions;
using Entities = NSE.Order.Domain.Entities.Orders;

namespace NSE.Order.Application.Commands;

public class OrderCommandHandler : CommandHandler,
    IRequestHandler<AddOrderCommand, ValidationResult>
{
    private readonly IQueue _queue;
    private readonly IOrderRepository _orderRepository;
    private readonly IVoucherRepository _voucherRepository;

    public OrderCommandHandler(
        IVoucherRepository voucherRepository,
        IOrderRepository orderRepository,
        IQueue queue
    )
    {
        _voucherRepository = voucherRepository;
        _orderRepository = orderRepository;
        _queue = queue;
    }

    public async Task<ValidationResult> Handle(AddOrderCommand message, CancellationToken cancellationToken)
    {
        // command validation
        if (!message.IsValid()) return message.ValidationResult;

        // Map Order
        var order = MapOrder(message);

        // apply voucher, if exists
        if (!await ApplyVoucher(message, order)) return ValidationResult;

        // Validate order
        if (!IsOrderValid(order)) return ValidationResult;

        // pay the order
        if (!await DoPayment(order, message)) return ValidationResult;

        // If paid, authorize order!
        order.Authorize();

        // Adding event
        order.AddEvent(new OrderDoneEvent(order.Id, order.CustomerId));

        // Add Order Repository
        _orderRepository.Add(order);

        // Commiting order and voucher data
        return await PersistData(_orderRepository.UnitOfWork);
    }

    private Entities.Order MapOrder(AddOrderCommand message)
    {
        var address = new Entities.Address
        {
            StreetAddress = message.Address.StreetAddress,
            BuildingNumber = message.Address.BuildingNumber,
            SecondaryAddress = message.Address.SecondaryAddress,
            Neighborhood = message.Address.Neighborhood,
            ZipCode = message.Address.ZipCode,
            City = message.Address.City,
            State = message.Address.State
        };

        var order = new Entities.Order(
            message.CustomerId, 
            message.Amount,
            message.OrderItems.Select(OrderItemDto.ToOrderItem).ToList(),
            message.HasVoucher, 
            message.Discount
        );

        order.SetAddress(address);
        return order;
    }

    private async Task<bool> ApplyVoucher(AddOrderCommand message, Entities.Order order)
    {
        if (!message.HasVoucher) return true;

        var voucher = await _voucherRepository.GetVoucherByCode(message.Voucher);
        if (voucher == null)
        {
            AddError("Voucher not found!");
            return false;
        }

        var voucherValidation = new VoucherValidation().Validate(voucher);
        if (!voucherValidation.IsValid)
        {
            voucherValidation.Errors.ToList().ForEach(m => AddError(m.ErrorMessage));
            return false;
        }

        order.AssociateVoucher(voucher);
        voucher.DebitQuantity();

        _voucherRepository.Update(voucher);

        return true;
    }

    private bool IsOrderValid(Entities.Order order)
    {
        var orderAmount = order.Amount;
        var orderDiscount = order.Discount;

        order.CalculateOrderAmount();

        if (order.Amount != orderAmount)
        {
            AddError("The order total amount order is different from total amount of individual items");
            return false;
        }

        if (order.Discount != orderDiscount)
        {
            AddError("The amount sent is different from order amount");
            return false;
        }

        return true;
    }

    private async Task<bool> DoPayment(Entities.Order order, AddOrderCommand message)
    {
        var orderStarted = new OrderInitiatedIntegrationEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            Amount = order.Amount,
            PaymentType = 1, // CreditCard: fixed - change if we have more types
            Holder = message.Holder,
            CardNumber = message.CardNumber,
            ExpirationDate = message.ExpirationDate,
            SecurityCode = message.SecurityCode
        };
        
        var result = await _queue.RequestAsync<OrderInitiatedIntegrationEvent, ResponseMessage>(orderStarted);

        if (result.ValidationResult.IsValid) return true;

        result.ValidationResult.Errors.ForEach(error => AddError(error.ErrorMessage)); 

        return false;
    }
}