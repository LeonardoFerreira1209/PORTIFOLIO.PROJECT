using System.ComponentModel;

namespace APPLICATION.DOMAIN.ENUMS;

public enum EventStatus
{
    [Description("Processed")]
    Processed = 1,

    [Description("Unprocessed")]
    Unprocessed = 2,

    [Description("Failed")]
    Failed = 3
}
