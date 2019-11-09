﻿using System;
using System.Collections.Generic;
using System.Text;
using Totten.Solutions.WolfMonitor.Domain.Enums;

namespace Totten.Solutions.WolfMonitor.Domain.Exceptions
{
    public class ForbiddenException : BusinessException
    {
        public ForbiddenException() : base(ErrorCodes.Forbidden, "Não autorizado")
        {
        }
    }
}
