using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StarForce.Editor {
    public class SpriteItem {
        public string SpriteName { get; set; }
        public int AtlasId { get; set; }
    }

    public class SpriteCollector {
        private List<SpriteItem> spriteItems = new List<SpriteItem>();

        public void AddItem(string spriteName, int atlasId) {
            SpriteItem item = new SpriteItem { SpriteName = spriteName, AtlasId = atlasId };
            spriteItems.Add(item);
        }

        public void GenerateConfig(string outputPath) {
            using (StreamWriter writer = new StreamWriter(outputPath, false, Encoding.UTF8)) {
                writer.WriteLine("#\tSprite图集映射配置表\t");
                writer.WriteLine("#\tSpriteName\tAtlasId");
                writer.WriteLine("#\tstring\tint");
                writer.WriteLine("#\t图片名称\t图集编号");

                foreach (var item in spriteItems) {
                    writer.WriteLine($"\t{item.SpriteName}\t{item.AtlasId}");
                }
            }
        }
    }
}