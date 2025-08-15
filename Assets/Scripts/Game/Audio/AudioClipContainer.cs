using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Audio
{
    [System.Serializable]
    public class AudioEntry
    {
        public SfxId id;
        public AudioClip clip;
    }
    
    [CreateAssetMenu(menuName = "Game/Data/Audio/Container")]
    public class AudioClipContainer : ScriptableObject
    {
        public List<AudioEntry> clips;

        private Dictionary<SfxId, AudioClip> clipMap;

        public void Init()
        {
            clipMap = clips.ToDictionary(c => c.id, c => c.clip);
        }

        public AudioClip GetClip(SfxId id)
        {
            if (id == SfxId.None)
                return null;
            
            return clipMap.GetValueOrDefault(id);
        }
    }
}