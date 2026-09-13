using Domain.Enums;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Booking : Entity, IAggregateRoot
    {
        public Guid SeatId { get; private set; }
        public Seat Seat { get; private set; } = null!;

        public Guid AttendeeId { get; private set; }
        public Attendee Attendee { get; private set; } = null!;

        public Money Price { get; private set; }
        public DateTime BookingAt { get; private set; }
        public BookingStatus Status { get; private set; }
        public string? QRCode { get; private set; }
        public string? PaymentReferenceId { get; private set; }

        private Booking(
            Guid id,
            Guid seatId,
            Guid attendeeId,
            Money price,
            DateTime bookingAt,
            string? qrCode,
            string? paymentReferenceId) : base(id)
        {
            SeatId = seatId;
            AttendeeId = attendeeId;
            Price = price;
            BookingAt = bookingAt;
            Status = BookingStatus.Pending;
            QRCode = qrCode;
            PaymentReferenceId = paymentReferenceId;
        }

        private Booking() : base(Guid.Empty)
        {
            Price = Money.Zero;
            Status = BookingStatus.Pending;
        }

        public static Result<Booking> Create(
            Guid seatId,
            Guid attendeeId,
            Money price,
            string? qrCode = null,
            string? paymentReferenceId = null,
            Guid? id = null)
        {
            if (seatId == Guid.Empty)
                return Result.Failure<Booking>(Error.Create("Booking.SeatIdRequired", "Seat ID is required."));

            if (attendeeId == Guid.Empty)
                return Result.Failure<Booking>(Error.Create("Booking.AttendeeIdRequired", "Attendee ID is required."));

            if (!string.IsNullOrWhiteSpace(qrCode) && qrCode.Length > Constants.Booking.QRCodeMaxLength)
                return Result.Failure<Booking>(Error.Create("Booking.QRCodeTooLong", $"QR code string cannot exceed {Constants.Booking.QRCodeMaxLength} characters."));

            if (!string.IsNullOrWhiteSpace(paymentReferenceId) && paymentReferenceId.Length > Constants.Booking.PaymentReferenceIdMaxLength)
                return Result.Failure<Booking>(Error.Create("Booking.PaymentReferenceIdTooLong", $"Payment reference ID cannot exceed {Constants.Booking.PaymentReferenceIdMaxLength} characters."));

            var booking = new Booking(
                id ?? Guid.NewGuid(),
                seatId,
                attendeeId,
                price,
                DateTime.UtcNow,
                qrCode?.Trim(),
                paymentReferenceId?.Trim());

            return booking;
        }

        public Result CompleteBooking(string paymentReferenceId, string? qrCode = null)
        {
            if (Status != BookingStatus.Pending)
                return Result.Failure(Error.Create("Booking.InvalidStatusTransition", "Only pending bookings can be completed."));

            if (string.IsNullOrWhiteSpace(paymentReferenceId))
                return Result.Failure(Error.Create("Booking.PaymentReferenceIdRequired", "Payment reference ID is required to complete booking."));

            Status = BookingStatus.Completed;
            PaymentReferenceId = paymentReferenceId.Trim();

            if (!string.IsNullOrWhiteSpace(qrCode))
                QRCode = qrCode.Trim();

            return Result.Success();
        }

        public Result CancelBooking()
        {
            if (Status == BookingStatus.Canceled)
                return Result.Failure(Error.Create("Booking.AlreadyCanceled", "Booking is already canceled."));

            Status = BookingStatus.Canceled;
            return Result.Success();
        }
    }
}
