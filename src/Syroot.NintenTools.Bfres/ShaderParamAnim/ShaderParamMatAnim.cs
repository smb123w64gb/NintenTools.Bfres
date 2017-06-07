using System;
using System.Collections.Generic;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    /// <summary>
    /// Represents a material parameter animation in a <see cref="ShaderParamAnim"/> subfile.
    /// </summary>
    public class ShaderParamMatAnim : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public int BeginCurve { get; set; }

        public int BeginParamAnim { get; set; }

        public string Name { get; set; }

        public IList<ParamAnimInfo> ParamAnimInfos { get; private set; }
        
        public IList<AnimCurve> Curves { get; private set; }

        public IList<AnimConstant> Constants { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ShaderParamMatAnimHead head = new ShaderParamMatAnimHead(loader);
            using (loader.TemporarySeek())
            {
                BeginCurve = head.BeginCurve;
                BeginParamAnim = head.BeginParamAnim;
                Name = loader.GetName(head.OfsName);
                ParamAnimInfos = loader.LoadList<ParamAnimInfo>(head.OfsParamAnimInfoList, head.NumAnimParam);
                Curves = loader.LoadList<AnimCurve>(head.OfsCurveList, head.NumCurve);

                if (head.OfsConstantList != 0)
                {
                    loader.Position = head.OfsConstantList;
                    Constants = loader.ReadAnimConstants(head.NumConstant);
                }
            }
        }

        void IResData.Reference(ResFileLoader loader)
        {
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="ShaderParamMatAnim"/> instance.
    /// </summary>
    internal class ShaderParamMatAnimHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal ushort NumAnimParam;
        internal ushort NumCurve;
        internal ushort NumConstant;
        internal int BeginCurve;
        internal int BeginParamAnim;
        internal uint OfsName;
        internal uint OfsParamAnimInfoList;
        internal uint OfsCurveList;
        internal uint OfsConstantList;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal ShaderParamMatAnimHead(ResFileLoader loader)
        {
            NumAnimParam = loader.ReadUInt16();
            NumCurve = loader.ReadUInt16();
            NumConstant = loader.ReadUInt16();
            loader.Seek(2);
            BeginCurve = loader.ReadInt32();
            BeginParamAnim = loader.ReadInt32();
            OfsName = loader.ReadOffset();
            OfsParamAnimInfoList = loader.ReadOffset();
            OfsCurveList = loader.ReadOffset();
            OfsConstantList = loader.ReadOffset();
        }
    }
}