﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet_App_Service.Models
{
    public class BaseResponse<T>
    {
        public bool IsSuccess { get; set; }
        public int HttpStatusCode { get; set; }
        public string ErrorInfo { get; set; }
        public T Result { get; set; }
    }
}
