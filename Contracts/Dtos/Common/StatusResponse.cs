namespace Contracts.Dtos.Common;

public class StatusResponse
{
    public string Status { get; set; } = null!;
    public string Message { get; set; } = null!;

    public static StatusResponse Error(string message) {
        return new StatusResponse {
            Status = "Error",
            Message = message
        };
    }

    public static StatusResponse Success(string message) {
        return new StatusResponse {
            Status = "Success",
            Message = message
        };
    }

    public static StatusResponse Warning(string message) {
        return new StatusResponse {
            Status = "Warning",
            Message = message
        };
    }
}