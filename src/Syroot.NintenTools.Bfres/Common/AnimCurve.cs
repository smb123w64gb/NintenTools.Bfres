using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an animation curve used by several sections to control different parameters over time.
    /// </summary>
    public class AnimCurve : IResData
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const ushort _flagsMaskFrameType = 0b00000000_00000011;
        private const ushort _flagsMaskKeyType = 0b00000000_00001100;
        private const ushort _flagsMaskCurveType = 0b00000000_01110000;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private ushort _flags;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Gets or sets the data type in which <see cref="Frames"/> are loaded and saved. For simplicity, the class
        /// always stores frames as converted <see cref="Single"/> instances.
        /// </summary>
        public AnimCurveFrameType FrameType
        {
            get { return (AnimCurveFrameType)(_flags & _flagsMaskFrameType); }
            set { _flags &= (ushort)(~_flagsMaskFrameType | (ushort)value); }
        }

        /// <summary>
        /// Gets or sets the data type in which <see cref="Keys"/> are loaded and saved. For simplicity, the class
        /// always stores frames as converted <see cref="Single"/> instances.
        /// </summary>
        public AnimCurveKeyType KeyType
        {
            get { return (AnimCurveKeyType)(_flags & _flagsMaskKeyType); }
            set { _flags &= (ushort)(~_flagsMaskKeyType | (ushort)value); }
        }

        /// <summary>
        /// Gets or sets the curve type, determining the number of elements stored with each key.
        /// </summary>
        public AnimCurveType CurveType
        {
            get { return (AnimCurveType)(_flags & _flagsMaskCurveType); }
            set { _flags &= (ushort)(~_flagsMaskCurveType | (ushort)value); }
        }

        public uint AnimDataOffset { get; set; }

        public float StartFrame { get; set; }

        public float EndFrame { get; set; }

        public float Scale { get; set; }

        public DWord Offset { get; set; }

        public float Delta { get; set; }

        public float[] Frames { get; set; }

        public float[] Keys { get; set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the number of elements stored for each key in <see cref="Keys"/>.
        /// </summary>
        /// <returns>The number of elements for each key in <see cref="Keys"/>.</returns>
        public int GetElementsPerKey()
        {
            switch (CurveType)
            {
                case AnimCurveType.Cubic:
                    return 4;
                case AnimCurveType.Linear:
                    return 2;
                default:
                    return 1;
            }
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            AnimCurveHead head = new AnimCurveHead(loader);
            using (loader.TemporarySeek())
            {
                _flags = head.Flags;
                AnimDataOffset = head.AnimDataOffset;
                StartFrame = head.FrameStart;
                EndFrame = head.FrameEnd;
                Scale = head.Scale;
                Offset = head.Offset;
                Delta = head.Delta;

                loader.Position = head.OfsFrameList;
                switch (FrameType)
                {
                    case AnimCurveFrameType.Single:
                        Frames = loader.ReadSingles(head.NumKey);
                        break;
                    case AnimCurveFrameType.Int16:
                        Frames = new float[head.NumKey];
                        for (int i = 0; i < head.NumKey; i++)
                        {
                            Frames[i] = loader.ReadInt16();
                        }
                        break;
                    case AnimCurveFrameType.Byte:
                        Frames = new float[head.NumKey];
                        for (int i = 0; i < head.NumKey; i++)
                        {
                            Frames[i] = loader.ReadByte();
                        }
                        break;
                }

                loader.Position = head.OfsKeyList;
                int keyElementCount = head.NumKey * GetElementsPerKey();
                switch (KeyType)
                {
                    case AnimCurveKeyType.Single:
                        Keys = loader.ReadSingles(keyElementCount);
                        break;
                    case AnimCurveKeyType.Int16:
                        Keys = new float[keyElementCount];
                        for (int i = 0; i < head.NumKey; i++)
                        {
                            Keys[i] = loader.ReadInt16();
                        }
                        break;
                    case AnimCurveKeyType.Byte:
                        Keys = new float[keyElementCount];
                        for (int i = 0; i < head.NumKey; i++)
                        {
                            Keys[i] = loader.ReadByte();
                        }
                        break;
                }
            }
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="AnimCurve"/> instance.
    /// </summary>
    internal class AnimCurveHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal ushort Flags;
        internal ushort NumKey;
        internal uint AnimDataOffset;
        internal float FrameStart;
        internal float FrameEnd;
        internal float Scale;
        internal DWord Offset;
        internal float Delta; // 3.4.0.0+
        internal uint OfsFrameList;
        internal uint OfsKeyList;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal AnimCurveHead(ResFileLoader loader)
        {
            Flags = loader.ReadUInt16();
            NumKey = loader.ReadUInt16();
            AnimDataOffset = loader.ReadUInt32();
            FrameStart = loader.ReadSingle();
            FrameEnd = loader.ReadSingle();
            Scale = loader.ReadSingle();
            Offset = loader.ReadSingle();
            if (loader.ResFile.Version >= 0x03040000)
            {
                Delta = loader.ReadSingle();
            }
            OfsFrameList = loader.ReadOffset();
            OfsKeyList = loader.ReadOffset();
        }
    }

    public enum AnimCurveFrameType : ushort
    {
        Single,
        Int16,
        Byte
    }

    public enum AnimCurveKeyType : ushort
    {
        Single = 0 << 2,
        Int16 = 1 << 2,
        Byte = 2 << 2
    }

    public enum AnimCurveType : ushort
    {
        Cubic = 0 << 4,
        Linear = 1 << 4,
        BakedFloat = 2 << 4,
        StepInt = 4 << 4,
        BakedInt = 5 << 4,
        StepBool = 6 << 4,
        BakedBool = 7 << 4
    }

    public struct AnimConstant
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        public uint TargetOffset;
        public DWord Value;
    }
}