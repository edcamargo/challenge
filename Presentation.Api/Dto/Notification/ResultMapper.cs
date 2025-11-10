using System.Collections.Generic;

namespace Presentation.Api.Dto
{
    public class ResultMapper<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public IEnumerable<NotificationDto>? Errors { get; set; }

        public static ResultMapper<T> Ok(T data) => new ResultMapper<T> { Success = true, Data = data };
        public static ResultMapper<T> Fail(IEnumerable<NotificationDto> errors) => new ResultMapper<T> { Success = false, Errors = errors };
    }
}

