// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace Game.TheLastOne
{

using global::System;
using global::FlatBuffers;

public struct Game_Items : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static Game_Items GetRootAsGame_Items(ByteBuffer _bb) { return GetRootAsGame_Items(_bb, new Game_Items()); }
  public static Game_Items GetRootAsGame_Items(ByteBuffer _bb, Game_Items obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public Game_Items __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public Gameitem? Data(int j) { int o = __p.__offset(4); return o != 0 ? (Gameitem?)(new Gameitem()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int DataLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<Game_Items> CreateGame_Items(FlatBufferBuilder builder,
      VectorOffset dataOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Game_Items.AddData(builder, dataOffset);
    return Game_Items.EndGame_Items(builder);
  }

  public static void StartGame_Items(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddData(FlatBufferBuilder builder, VectorOffset dataOffset) { builder.AddOffset(0, dataOffset.Value, 0); }
  public static VectorOffset CreateDataVector(FlatBufferBuilder builder, Offset<Gameitem>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDataVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Game_Items> EndGame_Items(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Game_Items>(o);
  }
};


}
