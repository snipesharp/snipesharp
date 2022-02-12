namespace Cli.Animatables
{
    public class Animatable
    {
        Task loop;
        int frame = -1;
        int framesCount = 0;
        Action<int> onFrame;
        CancellationTokenSource cancelation;
        CancellationToken cancelToken;

        public Animatable(int framesCount, Action<int> onFrame, int delayFrame, bool mainThread=false) {
            this.framesCount = framesCount;
            this.onFrame = onFrame;

            // setup cancelation
            this.cancelation = new CancellationTokenSource();
            this.cancelToken = cancelation.Token;

            // run main task and call each cycle
            this.loop = Task.Run(async () => {
                cancelToken.ThrowIfCancellationRequested();
                while (true) {
                    this.Cycle();
                    await Task.Delay(delayFrame, this.cancelation.Token);
                    if(cancelToken.IsCancellationRequested) cancelToken.ThrowIfCancellationRequested();
                }
            }, cancelation.Token);
        }

        public void Cycle() {
            if(++frame == framesCount) frame = 0;
            this.onFrame(frame);
        }

        public void Cancel() {
            cancelation.Cancel();
        }
    }
}