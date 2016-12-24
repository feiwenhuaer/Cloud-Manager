using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CustomHttpRequest
{
    public class ReceiveHeaderIAsyncResult : IAsyncResult
    {
        public ReceiveHeaderIAsyncResult(Stream st)
        {
            AsyncState_ = st;
        }

        object AsyncState_;
        public object AsyncState
        {
            get
            {
                return AsyncState_;
            }
        }



        public WaitHandle AsyncWaitHandle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsCompleted
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
