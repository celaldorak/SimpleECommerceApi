namespace ECommerce.Application.Common
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        // Başarıyla dönen bir sonuç
        public static Result SuccessResult(string message) => new Result { Success = true, Message = message };

        // Hata durumunda dönen bir sonuç
        public static Result FailureResult(string message) => new Result { Success = false, Message = message };
    }
}