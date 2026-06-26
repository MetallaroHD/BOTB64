namespace BOTB64.Graphics.Animations
{
    public static class AnimationManager
    {
        private static List<Animation> Running = new();
        private static Queue<Animation> BlockingQueue = new();
        private static bool _isBlocking => Running.Any(a => a.IsBlocking);
        public static bool IsBlocked => _isBlocking;

        public static void Play(Animation anim)
        {
            if (anim.IsBlocking)
            {
                // If something is already blocking, queue it
                if (_isBlocking)
                    BlockingQueue.Enqueue(anim);
                else
                    StartAnimation(anim);
            }
            else
            {
                // Non-blocking animations always start immediately
                StartAnimation(anim);
            }
        }

        private static void StartAnimation(Animation anim)
        {
            anim.Start();
            Running.Add(anim);
        }

        public static void Update(float dt)
        {
            foreach (var anim in Running)
                anim.Update(dt);

            // Clean up completed animations
            var completed = Running.Where(a => a.IsComplete).ToList();
            foreach (var anim in completed)
            {
                anim.OnComplete();
                Running.Remove(anim);

                // If it was blocking, start next in queue
                if (anim.IsBlocking && BlockingQueue.Count > 0)
                    StartAnimation(BlockingQueue.Dequeue());
            }
        }
        public static void Clear()
        {
            Running.Clear();
            BlockingQueue.Clear();
        }
    }
}