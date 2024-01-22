using System;
using System.Collections.Generic;
namespace EventSystem {
    public struct Body {
        private int id;
        private Delegate handler;
        private MsgRegister register;

        public Body(int id, Delegate handler, MsgRegister register) {
            this.id = id;
            this.handler = handler;
            this.register = register;
        }

        public void Unregister() {
            register.Unregister(id, handler);
        }
    }

    public class MsgRegister : IDisposable {
        // private Clock clock;
        private bool isDispose;
        private List<Wrapper> removes;
        private Dictionary<int, List<Wrapper>> handlers;

        public MsgRegister() {
            isDispose = false;
            removes = new List<Wrapper>();
            handlers = new Dictionary<int, List<Wrapper>>();
            // ClockUtil.Instance.AlarmRepeat(0, 0.1f, OnCheckRemoves);
        }

        public void Dispose() {
            isDispose = true;
            removes.Clear();
            handlers.Clear();
            // StopClock();

            removes = null;
            handlers = null;
        }

        public void Register(int id, Action<Body> handler) {
            RegisterDelegate(id, handler);
        }

        public void Register<T>(int id, Action<Body, T> handler) {
            RegisterDelegate(id, handler);
        }

        public void Register<T1, T2>(int id, Action<Body, T1, T2> handler) {
            RegisterDelegate(id, handler);
        }

        public void Register<T1, T2, T3>(int id, Action<Body, T1, T2, T3> handler) {
            RegisterDelegate(id, handler);
        }

        public void Register<T1, T2, T3, T4>(int id, Action<Body, T1, T2, T3, T4> handler) {
            RegisterDelegate(id, handler);
        }

        public void Register<T1, T2, T3, T4, T5>(int id, Action<Body, T1, T2, T3, T4, T5> handler) {
            RegisterDelegate(id, handler);
        }

        public void Unregister(int id, Delegate handler) {
            if (handler == null) {
                return;
            }

            if (handlers.TryGetValue(id, out List<Wrapper> wps)) {
                int index = SearchWrapperIndex(wps, handler);
                if (index >= 0) {
                    Wrapper wp = wps[index];
                    wp.isRemove = true;
                    wps[index] = wp;
                    removes.Add(wp);
                }
            }
        }

        public void Dispatcher(int id) {
            if (handlers.TryGetValue(id, out List<Wrapper> wps)) {
                int length = wps.Count;
                for (int i = 0; i < length; ++i) {
                    wps[i].Invoke(this);
                }
            }
        }

        public void Dispatcher<T>(int id, T data) {
            if (handlers.TryGetValue(id, out List<Wrapper> wps)) {
                int length = wps.Count;
                for (int i = 0; i < length; ++i) {
                    wps[i].Invoke(this, data);
                }
            }
        }

        public void Dispatcher<T1, T2>(int id, T1 data1, T2 data2) {
            if (handlers.TryGetValue(id, out List<Wrapper> wps)) {
                int length = wps.Count;
                for (int i = 0; i < length; ++i) {
                    wps[i].Invoke(this, data1, data2);
                }
            }
        }

        public void Dispatcher<T1, T2, T3>(int id, T1 data1, T2 data2, T3 data3) {
            if (handlers.TryGetValue(id, out List<Wrapper> wps)) {
                int length = wps.Count;
                for (int i = 0; i < length; ++i) {
                    wps[i].Invoke(this, data1, data2, data3);
                }
            }
        }

        public void Dispatcher<T1, T2, T3, T4>(int id, T1 data1, T2 data2, T3 data3, T4 data4) {
            if (handlers.TryGetValue(id, out List<Wrapper> wps)) {
                int length = wps.Count;
                for (int i = 0; i < length; ++i) {
                    wps[i].Invoke(this, data1, data2, data3, data4);
                }
            }
        }

        public void Dispatcher<T1, T2, T3, T4, T5>(int id, T1 data1, T2 data2, T3 data3, T4 data4, T5 data5) {
            if (handlers.TryGetValue(id, out List<Wrapper> wps)) {
                int length = wps.Count;
                for (int i = 0; i < length; ++i) {
                    wps[i].Invoke(this, data1, data2, data3, data4, data5);
                }
            }
        }

        private void RegisterDelegate(int id, Delegate handler) {
            if (handler == null) {
                return;
            }

            List<Wrapper> wps;

            if (!handlers.TryGetValue(id, out wps)) {
                wps = new List<Wrapper>();
                handlers.Add(id, wps);
            }

            if (SearchWrapperIndex(wps, handler) == -1) {
                wps.Add(new Wrapper(id, handler));
            }
        }

        private int SearchWrapperIndex(List<Wrapper> list, Delegate handler) {
            int index = -1;
            int length = list.Count;
            for (int i = 0; i < length; ++i) {
                if (list[i].handler == handler) {
                    index = i;
                    break;
                }
            }

            return index;
        }

        // private void OnCheckRemoves() {
        //     if (isDispose) {
        //         StopClock();
        //         return;
        //     }
        //
        //     int length = removes.Count;
        //     if (length == 0) {
        //         return;
        //     }
        //
        //     for (int i = 0; i < length; ++i) {
        //         Wrapper wp = removes[i];
        //         if (handlers.TryGetValue(wp.id, out List<Wrapper> wps)) {
        //             int index = SearchWrapperIndex(wps, wp.handler);
        //             if (index >= 0) {
        //                 wps.RemoveAt(index);
        //             }
        //
        //             if (wps.Count == 0) {
        //                 handlers.Remove(wp.id);
        //             }
        //         }
        //     }
        //     removes.Clear();
        // }

        // private void StopClock() {
        //     if (clock != null) {
        //         ClockUtil.Instance.Stop(clock);
        //         clock = null;
        //     }
        // }

        private struct Wrapper {
            public int id;
            public bool isRemove;
            public Delegate handler;

            public Wrapper(int id, Delegate handler) {
                this.id = id;
                this.isRemove = false;
                this.handler = handler;
            }

            public void Invoke(MsgRegister register) {
                if (!isRemove) {
                    ((Action<Body>)handler).Invoke(new Body(id, handler, register));
                }
            }

            public void Invoke<T>(MsgRegister register, T data) {
                if (!isRemove) {
                    ((Action<Body, T>)handler).Invoke(new Body(id, handler, register), data);
                }
            }

            public void Invoke<T1, T2>(MsgRegister register, T1 data1, T2 data2) {
                if (!isRemove) {
                    ((Action<Body, T1, T2>)handler).Invoke(new Body(id, handler, register), data1, data2);
                }
            }

            public void Invoke<T1, T2, T3>(MsgRegister register, T1 data1, T2 data2, T3 data3) {
                if (!isRemove) {
                    ((Action<Body, T1, T2, T3>)handler).Invoke(new Body(id, handler, register), data1, data2, data3);
                }
            }

            public void Invoke<T1, T2, T3, T4>(MsgRegister register, T1 data1, T2 data2, T3 data3, T4 data4) {
                if (!isRemove) {
                    ((Action<Body, T1, T2, T3, T4>)handler).Invoke(new Body(id, handler, register), data1, data2, data3, data4);
                }
            }

            public void Invoke<T1, T2, T3, T4, T5>(MsgRegister register, T1 data1, T2 data2, T3 data3, T4 data4, T5 data5) {
                if (!isRemove) {
                    ((Action<Body, T1, T2, T3, T4, T5>)handler).Invoke(new Body(id, handler, register), data1, data2, data3, data4, data5);
                }
            }
        }
    }
}