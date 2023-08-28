using Abp.Application.Services;
using CloudinaryTest.CoreHelper;
using CloudinaryTest.Entities;

namespace CloudinaryTest.Common.Manager
{
    public class CommonManager
    {
        public List<long> GetAllNodeAndLeafIdById(long Id, List<CloudFolder> list, bool isGetParent = false)
        {
            var listIds = new List<long>();
            var PC = list.Where(x => x.Id == Id).FirstOrDefault();
            if (PC.IsLeaf || isGetParent)
            {
                listIds.AddRange(GetAllParentId(PC.Id, list));
            }
            if (PC.IsLeaf)
            {
                listIds.Add(PC.Id);
            }
            else
            {
                listIds.Add(PC.Id);
                var listPCs = list.Where(x => x.ParentId == Id).ToList();
                foreach (var child in listPCs)
                {
                    listIds.AddRange(GetAllChildId(child.Id, list));
                }
            }
            return listIds;
        }

        public List<long> GetAllParentId(long Id, List<CloudFolder> list)
        {
            var result = new List<long>();
            var item = list.Where(x => x.Id == Id).FirstOrDefault();
            if (item == null) { return result; }
            if (item != null && item.ParentId.HasValue)
            {
                result.AddRange(GetAllParentId((long)item.ParentId, list));
            }
            result.Add((long)item.Id);
            return result;
        }

        public List<long> GetAllChildId(long Id, List<CloudFolder> list)
        {
            var result = new List<long>();
            result.Add(Id);
            var items = list.Where(x => x.ParentId == Id).Select(x => x.Id).ToList();
            if (items.Count > 0) { result.AddRange(items); }
            items.ForEach(x =>
            {
                result.AddRange(GetAllChildId(x, list));
            });
            return result;
        }

    }
}
