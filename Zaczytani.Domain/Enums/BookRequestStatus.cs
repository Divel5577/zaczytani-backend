using System.ComponentModel;

namespace Zaczytani.Domain.Enums;

public enum BookRequestStatus
{
    [Description("Oczekujący")]
    Pending = 0,

    [Description("Zaakceptowany")]
    Accepted = 1,

    [Description("Odrzucony")]
    Rejected = 2,
}