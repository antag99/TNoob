// TNoob - Trivial tool for changing the "Expert Mode" flag of a world
using System;
using System.IO;
using System.Windows.Forms;

public static class Program {
    public static void Main(string[] args) {
        if (args.Length < 1) {
            MessageBox.Show("Drag and drop the world file!");
            return;
        }

#if EXPERT
        SetExpertMode(args[0], args[0], true);
#else
        SetExpertMode(args[0], args[0], false);
#endif
    }

    private static void SetExpertMode(string source, string dest, bool expertMode) {
        BinaryReader reader = new BinaryReader(new FileStream(source, FileMode.Open));
        int version = reader.ReadInt32();
        if (version < 149) {
            MessageBox.Show("Error: Outdated terraria version");
            return;
        }
        ulong magic = reader.ReadUInt64();
        if ((magic & 72057594037927935uL) != 27981915666277746uL) {
            MessageBox.Show("Error: Invalid header");
            return;
        }
        // Skip other file metadata...
        reader.ReadBytes(12);
        int positionCount = reader.ReadInt16();
        int afterMetadataPos = reader.ReadInt32();
        int afterHeaderPos = reader.ReadInt32();
        // Skip positions...
        reader.ReadBytes((positionCount - 2) * 4);
        // Skip frame importance...
        reader.ReadBytes(reader.ReadInt16() / 8 + 1);
        if (reader.BaseStream.Position != afterMetadataPos) {
            MessageBox.Show("After Metadata Position Mismatch: expected " +
                afterMetadataPos + ", was " + reader.BaseStream.Position);
            return;
        }
        // Skip the first part of the header...
        reader.ReadString();
        reader.ReadInt32();
        reader.ReadInt32();
        reader.ReadInt32();
        reader.ReadInt32();
        reader.ReadInt32();
        reader.ReadInt32();
        reader.ReadInt32();
        // Get the offset...
        long expertModeFlagOffset = reader.BaseStream.Position;
        bool wasExpertMode = reader.ReadBoolean();
        reader.Dispose();
        // Notify the user if the world is changed...
        if (wasExpertMode == expertMode) {
            MessageBox.Show(expertMode ? "World was already Expert Mode." : "World was already not Expert Mode.");
            return;
        }
        BinaryWriter writer = new BinaryWriter(new FileStream(dest, FileMode.Open));
        writer.BaseStream.Position = expertModeFlagOffset;
        writer.Write(expertMode);
        writer.Dispose();
        MessageBox.Show(expertMode ? "World is now Expert Mode!" : "World is no longer Expert Mode!");
    }
}
