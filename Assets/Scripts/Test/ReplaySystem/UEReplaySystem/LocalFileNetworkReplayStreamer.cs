using System;

namespace Test.ReplaySystem.UEReplaySystem {
    // 写入到本地文件的网络回放流
    public class LocalFileNetworkReplayStreamer : NetworkReplayStreamer {
        enum EQueuedLocalFileRequestType {
            StartRecording,
            WriteHeader,
            WritingHeader,
            WritingStream,
            StopRecording,
            StartPlayback,
            ReadingHeader,
            ReadingStream,
            EnumeratingStreams,
            WritingCheckpoint,
            ReadingCheckpoint,
            UpdatingEvent,
            EnumeratingEvents,
            RequestingEvent,
            StopStreaming,
            DeletingFinishedStream,
            RefreshingLiveStream,
            KeepReplay,
            RenameReplay,
            RenameReplayFriendlyName,
        }

        public override void StartStreaming() {
            // 注册数据请求委托
            // AddDelegateFileRequestToQueue(EQueuedLocalFileRequestType.StartRecording, );
        }

        private void AddDelegateFileRequestToQueue(EQueuedLocalFileRequestType RequestType, Action InFunction, Action InCompletionCallback) {
            AddGenericRequestToQueue();
        }

        private void AddGenericRequestToQueue() {

        }
    }
}