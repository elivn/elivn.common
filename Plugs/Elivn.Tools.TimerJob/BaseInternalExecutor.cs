using System.Threading;
using System.Threading.Tasks;

namespace Elivn.Tools.TimerJob
{ 
    /// <summary>
    /// 任务基类
    ///       如果执行时间过长，重复触发时 当前任务还在进行中，则不做任何处理
    /// </summary>
    public abstract class BaseInternalExecutor : IJobExecutor
    {
        private bool _isRunning = false;
        private bool _jobCommandStarted = false;

        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; protected set; }

        /// <summary>
        ///  运行状态
        /// </summary>
        public StatusFlag StatusFlag
        {
            get
            {
                if (_jobCommandStarted && _isRunning)
                    return StatusFlag.Running;
                if (_jobCommandStarted && !_isRunning)
                    return StatusFlag.Waiting;
                if (!_jobCommandStarted && _isRunning)
                    return StatusFlag.Stopping;
                return StatusFlag.Stopped;
            }
        }
        /// <summary>
        ///   开始任务
        /// </summary>
        public Task StartAsync()
        {
            return StartAsync(CancellationToken.None);
        }

        /// <summary>
        ///   开始任务
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //  任务依然在执行中，不需要再次唤起
            if (_isRunning)
                return;

            _isRunning = _jobCommandStarted = true;

            if (!cancellationToken.IsCancellationRequested)
            {
                await InternalStartJob(cancellationToken);
            }
            _isRunning = false;
        }

        internal abstract Task InternalStartJob(CancellationToken cancellationToken);

        /// <summary>
        /// 结束任务
        /// </summary>
        public Task StopAsync()
        {
            return StopAsync(CancellationToken.None);
        }

        /// <summary>
        /// 结束任务
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _jobCommandStarted = false;
            return OnStoped(cancellationToken);
        }


        /// <summary>
        ///  任务停止
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task OnStoped(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


    }
}
