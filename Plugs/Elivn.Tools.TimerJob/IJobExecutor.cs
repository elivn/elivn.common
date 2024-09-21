using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elivn.Tools.TimerJob
{
    /// <summary>
    ///  任务 提供者 接口
    /// </summary>
    public interface IJobExecutor
    {
        /// <summary>
        ///  运行状态
        /// </summary>
        StatusFlag StatusFlag { get; }

        /// <summary>
        ///  工作名称
        /// </summary>
        string JobName { get; }

        /// <summary>
        /// 开始任务
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken);

        /// <summary>
        ///  结束任务
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken);
    }

    public enum StatusFlag
    {
        Waiting,
        Running,
        Stopping,
        Stopped
    }

    internal class InternalExecutor : BaseJobExecutor
    {
        private readonly Func<CancellationToken, Task> _startAction;
        private readonly Func<CancellationToken, Task> _stopAction;
           
        /// <inheritdoc />
        public InternalExecutor(string jobName, Func<CancellationToken, Task> startAction, Func<CancellationToken, Task> stopAction)
            :base(jobName)
        {
            _startAction = startAction;
            _stopAction = stopAction;
        }
            
        protected override Task OnStarted(CancellationToken cancellationToken)
        {
            return _startAction?.Invoke(cancellationToken) ?? Task.CompletedTask;
        }

        protected override Task OnStoped(CancellationToken cancellationToken)
        {
            return _stopAction?.Invoke(cancellationToken) ?? Task.CompletedTask;
        }
      
    }
}
