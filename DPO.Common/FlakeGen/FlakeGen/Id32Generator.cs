using System;
using System.Collections.Generic;
using System.Threading;

namespace FlakeGen
{
    /// <summary>
    /// Generated id is composed of
    /// <list type="bullet">
    /// <item><description>time - 28 bits (seconds precision w/ a custom epoch gives us 69 years)</description></item>
    /// <item><description>configured machine id - 1 bits (1 bit worker id, 1 bits datacenter id) - gives us up to 1 machines</description></item>
    /// <item><description>sequence number - 2 bits - rolls over every 2 per machine (with protection to avoid rollover in the same second)</description></item>
    /// </list>
    /// </summary>
    public class Id32Generator : IIdGenerator<int>
    {
        #region Private Constant

        /// <summary>
        /// 1 January 1970. Used to calculate timestamp (in milliseconds)
        /// </summary>
        private static readonly DateTime Jan1st2014 = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private const int Epoch = 0;

        /// <summary>
        /// Number of bits allocated for a worker id in the generated identifier. 5 bits indicates values from 0 to 31
        /// </summary>
        private const int WorkerIdBits = 1;

        /// <summary>
        /// Datacenter identifier this worker belongs to. 5 bits indicates values from 0 to 31
        /// </summary>
        private const int DatacenterIdBits = 0;

        /// <summary>
        /// Generator identifier. 10 bits indicates values from 0 to 1023
        /// </summary>
        private const int GeneratorIdBits = 2;

        /// <summary>
        /// Maximum generator identifier
        /// </summary>
        private const int MaxGeneratorId = -1 ^ (-1 << GeneratorIdBits);

        /// <summary>
        /// Maximum worker identifier
        /// </summary>
        private const int MaxWorkerId = -1 ^ (-1 << WorkerIdBits);

        /// <summary>
        /// Maximum datacenter identifier
        /// </summary>
        private const int MaxDatacenterId = -1 ^ (-1 << DatacenterIdBits);

        /// <summary>
        /// Number of bits allocated for sequence in the generated identifier
        /// </summary>
        private const int SequenceBits = 2;

        private const int WorkerIdShift = SequenceBits;

        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;

        private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

        private const int SequenceMask = -1 ^ (-1 << SequenceBits);

        #endregion Private Constant

        #region Private Fields

        /// <summary>
        /// Object used as a monitor for threads synchronization.
        /// </summary>
        private readonly object monitor = new object();

        /// <summary>
        /// The timestamp used to generate last id by the worker
        /// </summary>
        private int lastTimestamp = -1;

        private int sequence = 3;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Indicates how many times the given generator had to wait 
        /// for next millisecond <see cref="TillNext"/> since startup.
        /// </summary>
        public int NextSecondWait { get; set; }

        #endregion Public Properties

        #region Public Constructors

        public Id32Generator(int generatorId = 0, int sequence = 0)
            : this(
                (int)(generatorId & MaxWorkerId),
                (int)(generatorId >> WorkerIdBits & MaxDatacenterId),
                sequence)
        {
            // sanity check for generatorId
            if (generatorId > MaxGeneratorId || generatorId < 0)
            {
                throw new InvalidOperationException(
                    string.Format("Generator Id can't be greater than {0} or less than 0", MaxGeneratorId));
            }
        }

        public Id32Generator(int workerId, int datacenterId, int sequence = 0)
        {
            // sanity check for workerId
            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new InvalidOperationException(
                    string.Format("Worker Id can't be greater than {0} or less than 0", MaxWorkerId));
            }

            // sanity check for datacenterId
            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new InvalidOperationException(
                    string.Format("Datacenter Id can't be greater than {0} or less than 0", MaxDatacenterId));
            }

            WorkerId = workerId;
            DatacenterId = datacenterId;
            this.sequence = sequence;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The identifier of the worker
        /// </summary>
        public int WorkerId { get; private set; }

        /// <summary>
        /// Identifier of datacenter the worker belongs to
        /// </summary>
        public int DatacenterId { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public int GenerateId()
        {
            lock (monitor)
            {
                return NextId();
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            while (true)
            {
                yield return GenerateId();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        #endregion Public Methods

        #region Private Properties

        private static int CurrentTime
        {
            get { return (int)((DateTime.UtcNow - Jan1st2014).TotalMilliseconds / 10); }
        }

        #endregion Private Properties

        #region Public Methods
        #endregion Public Methods

        #region Private Static Methods
        #endregion Private Static Methods

        #region Private Methods

        private int TillNext(int lastTimestamp)
        {
            NextSecondWait++;

            var timestamp = CurrentTime;

            SpinWait.SpinUntil(() => (timestamp = CurrentTime) > lastTimestamp);

            return timestamp;
        }

        private int NextId()
        {
            var timestamp = CurrentTime;

            if (timestamp < lastTimestamp)
            {
                throw new InvalidOperationException(string.Format("Clock moved backwards. Refusing to generate id for {0} milliseconds", (lastTimestamp - timestamp)));
            }

            if (lastTimestamp == timestamp)
            {
                sequence = (sequence + 1) & SequenceMask;
                if (sequence == 0)
                {
                    timestamp = TillNext(lastTimestamp);
                }
            }
            else
            {
                sequence = 0;
            }

            lastTimestamp = timestamp;
            return ((timestamp - Epoch) << TimestampLeftShift) |
                (DatacenterId << DatacenterIdShift) |
                (WorkerId << WorkerIdShift) |
                sequence;
        }

        #endregion Private Methods
    }
}
