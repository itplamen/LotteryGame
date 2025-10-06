namespace WagerService.Core.NumberGenerators
{
    using Microsoft.Extensions.Configuration;

    using WagerService.Core.Contracts;
    
    public class SnowflakeNumberGenerator : INumberGeneration
    {
        private const int NODE_ID_BITS = 10;
        private const int SEQUENCE_BITS = 12;
        private const int MAX_NODE_ID = -1 ^ (-1 << NODE_ID_BITS);
        private const int MAX_SEQUENCE = -1 ^ (-1 << SEQUENCE_BITS);

        private readonly object lockObj = new();
        private readonly long nodeId;
        private readonly long epoch;
        private long lastTimestamp = -1;
        private long sequence = 0;

        public SnowflakeNumberGenerator(IConfiguration config)
        {
            string customEpoch = config["Snowflake:Epoch"];
            nodeId = long.Parse(config["Snowflake:NodeId"]);
            epoch = DateTimeOffset.Parse(customEpoch).ToUnixTimeMilliseconds();

            if (nodeId < 0 || nodeId > MAX_NODE_ID)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeId), $"NodeId must be between 0 and {MAX_NODE_ID}");
            }
        }

        public string Generate()
        {
            lock (lockObj)
            {
                long timestamp = GetCurrentTimestamp();

                if (timestamp < lastTimestamp)
                {
                    timestamp = WaitNextMillisecond(lastTimestamp);
                }

                if (timestamp == lastTimestamp)
                {
                    sequence = (sequence + 1) & MAX_SEQUENCE;

                    if (sequence == 0)
                    {
                        timestamp = WaitNextMillisecond(lastTimestamp);
                    }
                }
                else
                {
                    sequence = 0;
                }

                lastTimestamp = timestamp;

                long number = ((timestamp - epoch) << (NODE_ID_BITS + SEQUENCE_BITS))
                          | (nodeId << SEQUENCE_BITS)
                          | sequence;

                return number.ToString("D19");
            }
        }

        private long GetCurrentTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        private long WaitNextMillisecond(long lastTimestamp)
        {
            long timestamp;

            do
            {
                timestamp = GetCurrentTimestamp();
            } while (timestamp <= lastTimestamp);

            return timestamp;
        }
    }
}
