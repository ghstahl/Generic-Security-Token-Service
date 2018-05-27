namespace P7.Core.Localization
{
    public class ResourceQueryHandle
    {
        public string Id { get; set; }
        public string Treatment { get; set; }
        public string Culture { get; set; }
        public ResourceQueryHandle()
        {
        }

        public ResourceQueryHandle(ResourceQueryHandle doc)
        {
            this.Id = doc.Id;
            this.Treatment = doc.Treatment;
            this.Culture = doc.Culture;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ResourceQueryHandle;
            if (other == null)
            {
                return false;
            }

            return Id.Equals(other.Id)
                   && Treatment.Equals(other.Treatment)
                   && Culture.Equals(other.Culture);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}