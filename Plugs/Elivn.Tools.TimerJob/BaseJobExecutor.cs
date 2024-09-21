using System.Threading;
using System.Threading.Tasks;

namespace Elivn.Tools.TimerJob
{ 
    /// <summary>
    /// 任务基类
    ///       如果执行时间过长，重复触发时 当前任务还在进行中，则不做任何处理
    /// </summary>
    public abstract class BaseJobExecutor : BaseInternalExecutor
    {
        /// <summary>
        ///  列表任务执行者
        /// </summary>
        protected BaseJobExecutor(string jobName) 
        {
            JobName = jobName;
        }

        internal override Task InternalStartJob(CancellationToken cancellationToken)
        {
            return OnStarted(cancellationToken);
        }

        /// <summary>
        ///  任务执行
        /// </summary>
        protected abstract Task OnStarted(CancellationToken cancellationToken);

    }
}
