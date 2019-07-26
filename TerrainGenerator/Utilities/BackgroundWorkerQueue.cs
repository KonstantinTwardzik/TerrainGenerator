using System.Collections.Generic;
using System.ComponentModel;
/// <summary>
/// Simplifies running a Queue of BackgroundWorkers one after the other without blocking the current thread. 
/// </summary>
public class BackgroundWorkerQueue
{
    /// <summary>Continue running BackgroundWorkerQueue if any BackgroundWorker causes an Exception.</summary>
    public bool ContinueOnError { get; set; }
    private Queue<QueuedWorker> Queue { get; set; }
    public BackgroundWorkerQueue()
    {
        this.Queue = new Queue<QueuedWorker>();
    }
    public static BackgroundWorkerQueue operator +(BackgroundWorkerQueue left, BackgroundWorker worker)
    {
        left.Add(worker);
        return left;
    }
    public static BackgroundWorkerQueue operator +(BackgroundWorkerQueue left, QueuedWorker worker)
    {
        left.Add(worker);
        return left;
    }
    /// <summary>Add a BackgroundWorker to the Queue</summary>
    /// <param name="worker">BackgroundWorker to call RunWorkerAsync() on.</param>
    /// <param name="argument">A parameter for use by the background operation to be executed in the System.ComponentModel.BackgroundWorker.DoWork event handler.</param>
    public void Add(BackgroundWorker worker, object argument)
    {
        this.Queue.Enqueue(new QueuedWorker(worker, argument));
    }
    /// <summary>Add a BackgroundWorker to the Queue</summary>
    /// <param name="worker">BackgroundWorker to call RunWorkerAsync() on.</param>
    public void Add(BackgroundWorker worker)
    {
        this.Queue.Enqueue(new QueuedWorker(worker, null));
    }
    /// <summary>Add a BackgroundWorker to the Queue</summary>
    public void Add(QueuedWorker worker)
    {
        this.Queue.Enqueue(worker);
    }
    /// <summary>Starts execution of the BackgroundWorkers.</summary>
    public void Run()
    {
        //Debug.Print("BackgroundWorkerQueue.Run(), {0} items in queue.", this.Queue.Count);
        QueuedWorker q = this.Queue.Dequeue();
        q.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.Completed);
        q.Worker.RunWorkerAsync(q.Argument);
    }
    private void Completed(object sender, RunWorkerCompletedEventArgs e)
    {
        //Debug.Print("BackgroundWorkerQueue.Completed()");
        BackgroundWorker worker = sender as BackgroundWorker;
        if (worker != null)
        {
            worker.RunWorkerCompleted -= this.Completed; // Unsubscribe to event
            if ((this.ContinueOnError || e.Error == null) && this.Queue.Count > 0)
                this.Run(); // Run the next worker. 
        }
    }
    /// <summary>Object containing a BackgroundWorker and optional Argument to be run.</summary>
    public class QueuedWorker
    {
        /// <summary></summary>A parameter for use by the background operation to be executed in the System.ComponentModel.BackgroundWorker.DoWork event handler.</summary>
        public object Argument { get; set; }
        /// <summary>BackgroundWorker to be run.</summary>
        public BackgroundWorker Worker { get; set; }
        public QueuedWorker()
        {
        }
        /// <param name="worker">BackgroundWorker to call RunWorkerAsync() on.</param>
        /// <param name="argument">A parameter for use by the background operation to be executed in the System.ComponentModel.BackgroundWorker.DoWork event handler.</param>
        public QueuedWorker(BackgroundWorker worker, object argument)
        {
            this.Worker = worker;
            this.Argument = argument;
        }
    }
}