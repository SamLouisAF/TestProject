using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Sound Set", menuName = "Audio/Sound Set")]
public class SoundSet : ScriptableObject
{

    public List<Sound> sounds;
}
