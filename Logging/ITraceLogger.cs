using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeeUtils.Logging
{
    //TODO: consider better/simpler logging using: https://github.com/ninject/ninject.extensions.logging/wiki/Using
    public interface ITraceLogger
        {
            void Debug(string message);

            void Debug(string message, params object[] args);

            void Info(string message);

            void Info(string message, params object[] args);

            void Verbose(string message, params object[] args);

            void Warn(string message);

            void Warn(string message, params object[] args);

            void Error(string message);

            void Error(string message, params object[] args);

        }
    
}
