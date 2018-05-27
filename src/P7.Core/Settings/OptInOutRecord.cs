using System.Collections.Generic;

namespace P7.Core.Settings
{
    public class OptInOutRecord
    {
        public List<AreaNode> RouteTree { get; set; }

        public List<string> Routes { get; set; }
        public List<string> Areas { get; set; }

        private Dictionary<string, bool> _areasMap;
        public Dictionary<string, bool> AreasMap
        {
            get
            {
                if (_areasMap == null)
                {
                    _areasMap = new Dictionary<string, bool>();
                    if (Areas != null)
                    {
                        foreach (var item in Areas)
                        {
                            _areasMap.Add(item, true);
                        }
                    }
                }
                return _areasMap;
            }
        }

        public List<string> Controllers { get; set; }
        private Dictionary<string, bool> _controllersMap;
        public Dictionary<string, bool> ControllersMap
        {
            get
            {
                if (_controllersMap == null)
                {
                    _controllersMap = new Dictionary<string, bool>();
                    if (Controllers != null)
                    {
                        foreach (var item in Controllers)
                        {
                            _controllersMap.Add(item, true);
                        }
                    }
                }
                return _controllersMap;
            }
        }

        public List<string> Actions { get; set; }
        private Dictionary<string, bool> _actionsMap;
        public Dictionary<string, bool> ActionsMap
        {
            get
            {
                if (_actionsMap == null)
                {
                    _actionsMap = new Dictionary<string, bool>();
                    if (Actions != null)
                    {
                        foreach (var item in Actions)
                        {
                            _actionsMap.Add(item, true);
                        }
                    }
                }
                return _actionsMap;
            }
        }
    }
}