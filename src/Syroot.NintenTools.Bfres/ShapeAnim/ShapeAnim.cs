using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents an FSHA subfile in a <see cref="ResFile"/>, storing shape animations of a <see cref="Model"/>
    /// instance.
    /// </summary>
    [DebuggerDisplay(nameof(ShapeAnim) + " {" + nameof(Name) + "}")]
    public class ShapeAnim : INamedResData
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private string _name;
        private uint _ofsBindModel;

        // ---- EVENTS -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Raised when the <see cref="Name"/> property was changed.
        /// </summary>
        public event EventHandler NameChanged;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// The name with which the instance can be referenced uniquely in <see cref="INamedResDataList{ShapeAnim}"/>
        /// instances.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (_name != value)
                {
                    _name = value;
                    NameChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string Path { get; set; }

        public ShapeAnimFlags Flags { get; set; }

        public int FrameCount { get; set; }

        public uint BakedSize { get; set; }

        public Model BindModel { get; set; }

        public ushort[] BindIndices { get; private set; }

        public IList<VertexShapeAnim> VertexShapeAnims { get; private set; }

        public INamedResDataList<UserData> UserData { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ShapeAnimHead head = new ShapeAnimHead(loader);
            Name = loader.GetName(head.OfsName);
            Path = loader.GetName(head.OfsPath);
            FrameCount = head.NumFrame;
            BakedSize = head.SizBaked;
            _ofsBindModel = head.OfsBindModel;

            if (head.OfsBindIndexList != 0)
            {
                loader.Position = head.OfsBindIndexList;
                BindIndices = loader.ReadUInt16s(head.NumVertexShapeAnim);
            }

            VertexShapeAnims = loader.LoadList<VertexShapeAnim>(head.OfsVertexShapeAnimList, head.NumVertexShapeAnim);
            UserData = loader.LoadDictList<UserData>(head.OfsUserDataDict);
        }

        void IResData.Reference(ResFileLoader loader)
        {
            BindModel = loader.GetData<Model>(_ofsBindModel);
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="ShapeAnim"/> instance.
    /// </summary>
    internal class ShapeAnimHead
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "FSHA";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Signature;
        internal uint OfsName;
        internal uint OfsPath;
        internal ShapeAnimFlags Flags;
        internal ushort NumUserData;
        internal int NumFrame;
        internal ushort NumVertexShapeAnim;
        internal ushort NumKeyShapeAnim;
        internal ushort NumCurve;
        internal uint SizBaked;
        internal uint OfsBindModel;
        internal uint OfsBindIndexList;
        internal uint OfsVertexShapeAnimList;
        internal uint OfsUserDataDict;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal ShapeAnimHead(ResFileLoader loader)
        {
            Signature = loader.ReadSignature(_signature);
            OfsName = loader.ReadOffset();
            OfsPath = loader.ReadOffset();
            Flags = loader.ReadEnum<ShapeAnimFlags>(true);
            NumUserData = loader.ReadUInt16();
            NumFrame = loader.ReadInt32();
            NumVertexShapeAnim = loader.ReadUInt16();
            NumKeyShapeAnim = loader.ReadUInt16();
            NumCurve = loader.ReadUInt16();
            loader.Seek(2);
            SizBaked = loader.ReadUInt32();
            OfsBindModel = loader.ReadOffset();
            OfsBindIndexList = loader.ReadOffset();
            OfsVertexShapeAnimList = loader.ReadOffset();
            OfsUserDataDict = loader.ReadOffset();
        }
    }

    [Flags]
    public enum ShapeAnimFlags : ushort
    {
        BakedCurve = 1 << 0,
        Looping = 1 << 2
    }
}