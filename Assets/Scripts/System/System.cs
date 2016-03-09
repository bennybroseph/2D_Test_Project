using UnityEngine;
using System;

namespace Bennybroseph
{
    namespace MySystem
    {
        [Serializable]
        abstract public class BaseVector2<T>
        {
            protected T m_x, m_y;
            protected int m_HashCode;

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

            public virtual void GenHashCode()
            {
                if (GetType() == typeof(BaseVector2<int>))
                {
                    BaseVector2<int> This = this as BaseVector2<int>;
                    m_HashCode = ((int)Math.Pow(This.m_x, 2) + (3 * This.m_x) + (2 * This.m_x * This.m_y) + This.m_y + (int)Math.Pow(This.m_y, 2)) / 2;
                }
                if (GetType() == typeof(BaseVector2<float>))
                {
                    BaseVector2<float> This = this as BaseVector2<float>;
                    m_HashCode = (int)(Math.Pow(This.m_x, 2) + (3 * This.m_x) + (2 * This.m_x * This.m_y) + This.m_y + (int)Math.Pow(This.m_y, 2)) / 2;
                }
            }

            public BaseVector2() { GenHashCode(); }
            public BaseVector2(T a_x, T a_y)
            {
                m_x = a_x;
                m_y = a_y;

                GenHashCode();
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
        [Serializable]
        public class IntVector2 : BaseVector2<int>
        {
            public int x
            {
                get { return m_x; }
                set { m_x = value; GenHashCode(); }
            }
            public int y
            {
                get { return m_y; }
                set { m_y = value; GenHashCode(); }
            }

            public IntVector2() : base() { }
            public IntVector2(int a_x, int a_y) : base(a_x, a_y) { }
        }
        [Serializable]
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