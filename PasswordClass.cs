using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates
{
    static class PasswordClass //Глобальный публичный клас для передачи пароля между формами если не хотим сохранять пароль
    {
        public static string Value { get; set; }
    }
}
