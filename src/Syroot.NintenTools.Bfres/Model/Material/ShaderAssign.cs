using System.Collections.Generic;
using Syroot.NintenTools.Bfres.Core;

namespace Syroot.NintenTools.Bfres
{
    public class ShaderAssign : IResData
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public string ShaderArchiveName { get; set; }

        public string ShadingModelName { get; set; }

        public uint Revision { get; set; }

        public IList<string> AttribAssigns { get; private set; }

        public IList<string> SamplerAssigns { get; private set; }

        public IList<string> ShaderOptions { get; private set; }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        void IResData.Load(ResFileLoader loader)
        {
            ShaderAssignHead head = new ShaderAssignHead(loader);
            ShaderArchiveName = loader.GetName(head.OfsShaderArchiveName);
            ShadingModelName = loader.GetName(head.OfsShadingModelName);
            Revision = head.Revision;
            AttribAssigns = loader.LoadDictNames(head.OfsAttribAssignDict);
            SamplerAssigns = loader.LoadDictNames(head.OfsSamplerAssignDict);
            ShaderOptions = loader.LoadDictNames(head.OfsShaderOptionDict);
        }
    }

    /// <summary>
    /// Represents the header of a <see cref="ShaderAssign"/> instance.
    /// </summary>
    internal class ShaderAssignHead
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint OfsShaderArchiveName;
        internal uint OfsShadingModelName;
        internal uint Revision;
        internal byte NumAttribAssign;
        internal byte NumSamplerAssign;
        internal ushort NumShaderOption;
        internal uint OfsAttribAssignDict;
        internal uint OfsSamplerAssignDict;
        internal uint OfsShaderOptionDict;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal ShaderAssignHead(ResFileLoader loader)
        {
            OfsShaderArchiveName = loader.ReadOffset();
            OfsShadingModelName = loader.ReadOffset();
            Revision = loader.ReadUInt32();
            NumAttribAssign = loader.ReadByte();
            NumSamplerAssign = loader.ReadByte();
            NumShaderOption = loader.ReadUInt16();
            OfsAttribAssignDict = loader.ReadOffset();
            OfsSamplerAssignDict = loader.ReadOffset();
            OfsShaderOptionDict = loader.ReadOffset();
        }
    }
}