using System;

namespace EventSystem {
    public class MessageSystem : MonoSingleton<MessageSystem> {
        private bool isDispose;
        private MsgRegister msgRegister;

        public MessageSystem() {
            isDispose = false;
            msgRegister = new MsgRegister();
        }

        ~MessageSystem() {
            isDispose = true;
            msgRegister = null;
        }

        public void DispatchMessage(int id) {
            if (isDispose) {
                return;
            }

            msgRegister.Dispatcher(id);
        }

        public void DispatchMessage<T>(int id, T data) {
            if (isDispose) {
                return;
            }

            msgRegister.Dispatcher(id, data);
        }

        public void DispatchMessage<T1, T2>(int id, T1 data1, T2 data2) {
            if (isDispose) {
                return;
            }

            msgRegister.Dispatcher(id, data1, data2);
        }

        public void RegisterMessage(int id, Action<Body> handler) {
            if (isDispose) {
                return;
            }

            msgRegister.Register(id, handler);
        }

        public void RegisterMessage<T>(int id, Action<Body, T> handler) {
            if (isDispose) {
                return;
            }

            msgRegister.Register(id, handler);
        }

        public void RegisterMessage<T1, T2>(int id, Action<Body, T1, T2> handler) {
            if (isDispose) {
                return;
            }

            msgRegister.Register(id, handler);
        }

        public void UnregisterMessage(int id, Action<Body> handler) {
            if (isDispose) {
                return;
            }

            msgRegister.Unregister(id, handler);
        }

        public void UnregisterMessage<T>(int id, Action<Body, T> handler) {
            if (isDispose) {
                return;
            }

            msgRegister.Unregister(id, handler);
        }

        public void UnregisterMessage<T1, T2>(int id, Action<Body, T1, T2> handler) {
            if (isDispose) {
                return;
            }

            msgRegister.Unregister(id, handler);
        }

        public void UnregisterMessage<T>(int id, Delegate handler) {
            if (isDispose) {
                return;
            }

            msgRegister.Unregister(id, handler);
        }
    }
}