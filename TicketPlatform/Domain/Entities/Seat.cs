using Domain.Enums;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Seat : Entity
    {
        public Guid EventId { get; private set; }
        public Event Event { get; private set; } = null!;

        public int Row { get; private set; }
        public int SeatNumber { get; private set; }
        public string? Type { get; private set; }
        public Money Price { get; private set; }
        public SeatStatus Status { get; private set; }
        public DateTime? LockExpiration { get; private set; }

        private Seat(
            Guid id,
            Guid eventId,
            int row,
            int seatNumber,
            Money price,
            string? type) : base(id)
        {
            EventId = eventId;
            Row = row;
            SeatNumber = seatNumber;
            Price = price;
            Type = type;
            Status = SeatStatus.Available;
        }

        private Seat() : base(Guid.Empty)
        {
            Price = Money.Zero;
            Status = SeatStatus.Available;
        }

        public static Result<Seat> Create(
            Guid eventId,
            int row,
            int seatNumber,
            Money price,
            string? type = null,
            Guid? id = null)
        {
            if (eventId == Guid.Empty)
                return Result.Failure<Seat>(Error.Create("Seat.EventIdRequired", "Event ID is required."));

            if (row < Constants.Seat.MinRow)
                return Result.Failure<Seat>(Error.Create("Seat.InvalidRow", $"Row number must be at least {Constants.Seat.MinRow}."));

            if (seatNumber < Constants.Seat.MinSeatNumber)
                return Result.Failure<Seat>(Error.Create("Seat.InvalidSeatNumber", $"Seat number must be at least {Constants.Seat.MinSeatNumber}."));

            if (!string.IsNullOrWhiteSpace(type) && type.Length > Constants.Seat.TypeMaxLength)
                return Result.Failure<Seat>(Error.Create("Seat.TypeTooLong", $"Seat type cannot exceed {Constants.Seat.TypeMaxLength} characters."));

            return new Seat(id ?? Guid.NewGuid(), eventId, row, seatNumber, price, type?.Trim());
        }

        public Result Lock(TimeSpan lockDuration)
        {
            if (Status == SeatStatus.Booked)
                return Result.Failure(Error.Create("Seat.AlreadyBooked", "Seat is already booked."));

            if (Status == SeatStatus.Locked && LockExpiration.HasValue && LockExpiration.Value > DateTime.UtcNow)
                return Result.Failure(Error.Create("Seat.AlreadyLocked", "Seat is currently locked by another user."));

            Status = SeatStatus.Locked;
            LockExpiration = DateTime.UtcNow.Add(lockDuration);
            return Result.Success();
        }

        public Result Book()
        {
            if (Status == SeatStatus.Booked)
                return Result.Failure(Error.Create("Seat.AlreadyBooked", "Seat is already booked."));

            Status = SeatStatus.Booked;
            LockExpiration = null;
            return Result.Success();
        }

        public Result ReleaseLock()
        {
            if (Status == SeatStatus.Booked)
                return Result.Failure(Error.Create("Seat.CannotReleaseBooked", "Cannot release lock on a booked seat."));

            Status = SeatStatus.Available;
            LockExpiration = null;
            return Result.Success();
        }
    }
}
