using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceProlongation.Model
{
    public sealed class Status
    {
        private readonly string _name;

        public static readonly Status Created = new Status("Создание заявки");
        public static readonly Status InProgress = new Status("Выполняются работы");
        public static readonly Status NeedSigning = new Status("Оформить и подписать");
        public static readonly Status SentToRst = new Status("Направлен в Росстандарт");
        public static readonly Status CameFromRst = new Status("Принят утвержденный компект");
        public static readonly Status Finished = new Status("Завершено");

        private Status(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        public static implicit operator string(Status s) => s._name;
    }
}
