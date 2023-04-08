using System.Collections.Generic;
using System.Linq;
using CizaAudioModule.Implement;
using Unity.VisualScripting;
using IInitializable = Zenject.IInitializable;

namespace CizaAudioModule.Example1
{
    // public class AudioModuleExampleController : IInitializable
    // {
    //     //private variable
    //     private AudioModule _audioModule;
    //     private AudioResourceDataOverview _audioResourceDataOverview;
    //     private ComponentCollectionData _componentCollectionData;
    //     
    //     private string _currentKey;
    //     private string _currentAudioId;
    //     private Dictionary<string, List<string>> _audioKeyIdsMaps = new Dictionary<string, List<string>>();
    //     
    //     //public method
    //     public AudioModuleExampleController(AudioModule audioModule,
    //         AudioResourceDataOverview audioResourceDataOverview, ComponentCollectionData componentCollectionData)
    //     {
    //         _audioModule = audioModule;
    //         _audioResourceDataOverview = audioResourceDataOverview;
    //         _componentCollectionData = componentCollectionData;
    //     }
    //     
    //     public void Initialize()
    //     {
    //         var audioResourceDatas = _audioResourceDataOverview.GetAudioDatas;
    //         _audioModule.Initialize(audioResourceDatas);
    //     
    //         _componentCollectionData.PlayButton.onClick.AddListener(() =>
    //         {
    //             _currentKey = _componentCollectionData.Key;
    //             _currentAudioId = _audioModule.Play(_currentKey, _componentCollectionData.AudioLocalPosition,
    //                 _componentCollectionData.AudioParentTransform);
    //             if(!_audioKeyIdsMaps.ContainsKey(_currentKey))
    //                 _audioKeyIdsMaps.Add(_currentKey, new List<string>());
    //     
    //             var ids = _audioKeyIdsMaps[_currentKey];
    //             ids.Add(_currentAudioId);
    //         });
    //         
    //         _componentCollectionData.PauseButton.onClick.AddListener(() =>
    //         {
    //             _audioModule.Pause(_currentAudioId);
    //         });
    //         
    //         _componentCollectionData.ResumeButton.onClick.AddListener(() =>
    //         {
    //             _audioModule.Resume(_currentAudioId);
    //         });
    //     
    //         _componentCollectionData.StopButton.onClick.AddListener(() =>
    //         {
    //             _audioModule.Stop(_currentAudioId);
    //             
    //             var ids = _audioKeyIdsMaps[_currentKey];
    //             ids.Remove(_currentAudioId);
    //         });
    //     
    //         _componentCollectionData.StopAllButton.onClick.AddListener(() =>
    //         {
    //             var idsMaps = _audioKeyIdsMaps.Values.ToArray();
    //             foreach (var ids in idsMaps)
    //             {
    //                 foreach (var id in ids.ToArray())
    //                 {
    //                     _audioModule.Stop(id);
    //                     ids.Remove(id);
    //                 }
    //             }
    //         });
    //         
    //         _componentCollectionData.ReleasePoolByKeyButton.onClick.AddListener(() =>
    //         {
    //             var key = _componentCollectionData.Key;
    //             _audioModule.ReleasePool(key);
    //             
    //             var ids = _audioKeyIdsMaps[key];
    //             if(ids.Contains(_currentAudioId)) 
    //                 _currentAudioId = string.Empty;
    //             
    //             _audioKeyIdsMaps.Remove(key);
    //         });
    //         
    //         _componentCollectionData.ReleaseAllPoolsButton.onClick.AddListener(() =>
    //         {
    //             _audioModule.ReleaseAllPools();
    //             _currentAudioId = string.Empty;
    //             _audioKeyIdsMaps.Clear();
    //         });
    //     
    //         _componentCollectionData.MasterSlider.onValueChanged.AddListener(masterVolume =>
    //             _audioModule.SetAudioMixerVolume(masterVolume));
    //         _componentCollectionData.BgmSlider.onValueChanged.AddListener(bgmVolume =>
    //             _audioModule.SetBGMVolume(bgmVolume));
    //         _componentCollectionData.SfxSlider.onValueChanged.AddListener(sfxVolume =>
    //             _audioModule.SetSFXVolume(sfxVolume));
    //         _componentCollectionData.VoiceSlider.onValueChanged.AddListener(voiceVolume =>
    //             _audioModule.SetVoiceVolume(voiceVolume));
    //     }
    // }
}