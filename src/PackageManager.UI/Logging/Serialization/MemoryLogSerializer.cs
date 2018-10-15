using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo;
using Neptuo.Logging;
using Neptuo.Logging.Serialization;
using Neptuo.Logging.Serialization.Formatters;

namespace PackageManager.Logging.Serialization
{
    public class MemoryLogSerializer : ILogSerializer
    {
        private readonly StringBuilder content = new StringBuilder();
        private readonly ILogFormatter formatter;

        public MemoryLogSerializer(ILogFormatter formatter)
        {
            Ensure.NotNull(formatter, "formatter");
            this.formatter = formatter;
        }

        public string GetContent() 
            => content.ToString();

        public void Clear()
            => content.Clear();

        public void Append(string scopeName, LogLevel level, object model)
            => content.AppendLine(formatter.Format(scopeName, level, model));

        public bool IsEnabled(string scopeName, LogLevel level) 
            => true;
    }
}
