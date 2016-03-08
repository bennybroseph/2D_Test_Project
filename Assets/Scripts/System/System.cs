using UnityEngine;
using System.Collections.Generic;
using System;

namespace Bennybroseph
{
    namespace System
    {
        abstract public class BaseVector2<T> : IEquatable<BaseVector2<T>>, IEqualityComparer<BaseVector2<T>>
        {
            protected T m_x, m_y;

            static public implicit operator Vector2(BaseVector2<T> a_BaseVector)
            {
                if (typeof(T) == typeof(int))
                {
                    BaseVector2<int> BaseVector = a_BaseVector as BaseVector2<int>;
                    return new Vector2(BaseVector.m_x, BaseVector.m_y);
                }
                else if (typeof(T) == typeof(float))
                {
                    BaseVector2<float> BaseVector = a_BaseVector as BaseVector2<float>;
                    return new Vector2(BaseVector.m_x, BaseVector.m_y);
                }
                else if (typeof(T) == typeof(long))
                {
                    BaseVector2<long> BaseVector = a_BaseVector as BaseVector2<long>;
                    return new Vector2(BaseVector.m_x, BaseVector.m_y);
                }
                else if (typeof(T) == typeof(double))
                {
                    BaseVector2<double> BaseVector = a_BaseVector as BaseVector2<double>;
                    return new Vector2((float)BaseVector.m_x, (float)BaseVector.m_y);
                }
                else
                    return new Vector2();
            }

            public override string ToString()
            {
                return "(" + m_x.ToString() + ", " + m_y.ToString() + ")";
            }

            public virtual bool Equals(BaseVector2<T> a_VectorX, BaseVector2<T> a_VectorY)
            {
                Debug.Log("Test");
                if (typeof(T) == typeof(int))
                {
                    BaseVector2<int> VectorX = a_VectorX as BaseVector2<int>;
                    BaseVector2<int> VectorY = a_VectorY as BaseVector2<int>;

                    if (VectorY.m_x == VectorX.m_x && VectorY.m_y == VectorX.m_y)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }

            public virtual int GetHashCode(BaseVector2<T> obj)
            {
                Debug.Log("Oops");
                throw new NotImplementedException();
            }

            public virtual bool Equals(BaseVector2<T> other)
            {
                Debug.Log("Test");
                if (typeof(T) == typeof(int))
                {
                    BaseVector2<int> VectorX = other as BaseVector2<int>;
                    BaseVector2<int> VectorY = this as BaseVector2<int>;

                    if (VectorY.m_x == VectorX.m_x && VectorY.m_y == VectorX.m_y)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }

            public BaseVector2() { }
            public BaseVector2(T a_x, T a_y)
            {
                m_x = a_x;
                m_y = a_y;
            }
        }
        abstract public class BaseVector3<T> : BaseVector2<T>
        {
            protected T m_z;

            public BaseVector3() : base() { }
            public BaseVector3(T a_x, T a_y, T a_z) : base(a_x, a_y)
            {
                m_z = a_z;
            }
        }

        public class Vector2<T> : BaseVector2<T>
        {
            public T x
            {
                get { return m_x; }
                set { m_x = value; }
            }
            public T y
            {
                get { return m_y; }
                set { m_y = value; }
            }

            public Vector2() : base() { }
            public Vector2(T a_x, T a_y) : base(a_x, a_y) { }
        }

        public class Vector3<T> : BaseVector3<T>
        {
            public T x
            {
                get { return m_x; }
                set { m_x = value; }
            }
            public T y
            {
                get { return m_y; }
                set { m_y = value; }
            }
            public T z
            {
                get { return m_z; }
                set { m_z = value; }
            }

            public Vector3() : base() { }
            public Vector3(T a_x, T a_y, T a_z) : base(a_x, a_y, a_z) { }
        }
    }
}