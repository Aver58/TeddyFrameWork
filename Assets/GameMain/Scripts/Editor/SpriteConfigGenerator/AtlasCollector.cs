using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StarForce.Editor {
    public class AtlasItem {
        public int Id { get; set; }
        public string AtlasName { get; set; }
    }

    // 收集图集映射
    public class AtlasCollector {
        private List<AtlasItem> atlasItems = new List<AtlasItem>();

        public void AddItem(int id, string atlasName) {
            AtlasItem item = new AtlasItem { Id = id, AtlasName = atlasName };
            atlasItems.Add(item);
        }

        public void GenerateConfig(string outputPath) {
            using (StreamWriter writer = new StreamWriter(outputPath, false, Encoding.UTF8)) {
                writer.WriteLine("#\tAtlas枚举配置表\t");
                writer.WriteLine("#\tId\tAtlasName");
                writer.WriteLine("#\tint\tstring");
                writer.WriteLine("#\t编号\t图集名称");

                foreach (var item in atlasItems) {
                    writer.WriteLine($"\t{item.Id}\t{item.AtlasName}");
                }
            }
        }
    }
}