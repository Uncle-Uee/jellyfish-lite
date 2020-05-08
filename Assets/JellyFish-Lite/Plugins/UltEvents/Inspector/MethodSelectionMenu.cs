// UltEvents // Copyright 2019 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UltEvents.Editor
{
    /// <summary>[Editor-Only]
    /// Manages the construction of menus for selecting methods for <see cref="PersistentCall"/>s.
    /// </summary>
    internal static class MethodSelectionMenu
    {
        /************************************************************************************************************************/
        #region Fields
        /************************************************************************************************************************/

        /// <summary>
        /// The drawer state from when the menu was opened which needs to be restored when a method is selected because
        /// menu items are executed after the frame finishes and the drawer state is cleared.
        /// </summary>
        private static readonly DrawerState
            CachedState = new DrawerState();

        private static readonly StringBuilder
            LabelBuilder = new StringBuilder();

        // These fields should really be passed around as parameters, but they make all the method signatures annoyingly long.
        private static MethodBase _CurrentMethod;
        private static BindingFlags _Bindings;
        private static GenericMenu _Menu;

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Entry Point
        /************************************************************************************************************************/

        /// <summary>Opens the menu near the specified 'area'.</summary>
        public static void ShowMenu(Rect area)
        {
            CachedState.CopyFrom(DrawerState.Current);

            _CurrentMethod = CachedState.call.GetMethodSafe();
            _Bindings = GetBindingFlags();
            _Menu = new GenericMenu();

            BoolPref.AddDisplayOptions(_Menu);

            Object[] targetObjects;
            Object[] targets = GetObjectReferences(CachedState.TargetProperty, out targetObjects);

            AddCoreItems(targets);

            // Populate the main contents of the menu.
            {
                if (targets == null)
                {
                    string serializedMethodName = CachedState.MethodNameProperty.stringValue;
                    Type declaringType;
                    string methodName;
                    PersistentCall.GetMethodDetails(serializedMethodName, null, out declaringType, out methodName);

                    // If we have no target, but do have a type, populate the menu with that type's statics.
                    if (declaringType != null)
                    {
                        PopulateMenuWithStatics(targetObjects, declaringType);

                        goto ShowMenu;
                    }

                    targets = targetObjects;
                }

                // Ensure that all targets share the same type.
                Object firstTarget = ValidateTargetsAndGetFirst(targets);
                if (firstTarget == null)
                {
                    targets = targetObjects;
                    firstTarget = targets[0];
                }

                // Add menu items according to the type of the target.
                if (firstTarget is GameObject)
                    PopulateMenuForGameObject("", false, targets);
                else if (firstTarget is Component)
                    PopulateMenuForComponent(targets);
                else
                    PopulateMenuForObject(targets);
            }

            ShowMenu:

            _Menu.DropDown(area);

            GC.Collect();
        }

        /************************************************************************************************************************/

        private static BindingFlags GetBindingFlags()
        {
            BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance;

            if (BoolPref.ShowNonPublicMethods)
                bindings |= BindingFlags.NonPublic;

            if (BoolPref.ShowStaticMethods)
                bindings |= BindingFlags.Static;

            return bindings;
        }

        /************************************************************************************************************************/

        private static void AddCoreItems(Object[] targets)
        {
            _Menu.AddItem(new GUIContent("Null"), _CurrentMethod == null, () =>
            {
                DrawerState.Current.CopyFrom(CachedState);

                if (targets != null)
                {
                    PersistentCallDrawer.SetMethod(null);
                }
                else
                {
                    // For a static method, remove the method name but keep the declaring type.
                    string methodName = CachedState.MethodNameProperty.stringValue;
                    int lastDot = methodName.LastIndexOf('.');
                    if (lastDot < 0)
                        CachedState.MethodNameProperty.stringValue = null;
                    else
                        CachedState.MethodNameProperty.stringValue = methodName.Substring(0, lastDot + 1);

                    CachedState.PersistentArgumentsProperty.arraySize = 0;

                    CachedState.MethodNameProperty.serializedObject.ApplyModifiedProperties();
                }

                DrawerState.Current.Clear();
            });

            bool isStatic = _CurrentMethod != null && _CurrentMethod.IsStatic;
            if (targets != null && !isStatic)
            {
                _Menu.AddItem(new GUIContent("Static Method"), isStatic, () =>
                {
                    DrawerState.Current.CopyFrom(CachedState);

                    PersistentCallDrawer.SetTarget(null);

                    DrawerState.Current.Clear();
                });
            }

            _Menu.AddSeparator("");
        }

        /************************************************************************************************************************/

        private static Object[] GetObjectReferences(SerializedProperty property, out Object[] targetObjects)
        {
            targetObjects = property.serializedObject.targetObjects;

            if (property.hasMultipleDifferentValues)
            {
                Object[] references = new Object[targetObjects.Length];
                for (int i = 0; i < references.Length; i++)
                {
                    using (SerializedObject serializedObject = new SerializedObject(targetObjects[i]))
                    {
                        references[i] = serializedObject.FindProperty(property.propertyPath).objectReferenceValue;
                    }
                }
                return references;
            }

            Object target = property.objectReferenceValue;
            if (target != null)
                return new[] { target };
            return null;

        }

        /************************************************************************************************************************/

        private static Object ValidateTargetsAndGetFirst(Object[] targets)
        {
            Object firstTarget = targets[0];
            if (firstTarget == null)
                return null;

            Type targetType = firstTarget.GetType();

            // Make sure all targets have the exact same type.
            // Unfortunately supporting inheritance would be more complicated.

            int i = 1;
            for (; i < targets.Length; i++)
            {
                Object obj = targets[i];
                if (obj == null || obj.GetType() != targetType)
                {
                    return null;
                }
            }

            return firstTarget;
        }

        /************************************************************************************************************************/

        private static T[] GetRelatedObjects<T>(Object[] objects, System.Func<Object, T> getRelatedObject)
        {
            T[] relatedObjects = new T[objects.Length];

            for (int i = 0; i < relatedObjects.Length; i++)
            {
                relatedObjects[i] = getRelatedObject(objects[i]);
            }

            return relatedObjects;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Populate for Objects
        /************************************************************************************************************************/

        private static void PopulateMenuWithStatics(Object[] targets, Type type)
        {
            Object firstTarget = targets[0];
            Component component = firstTarget as Component;
            if (!ReferenceEquals(component, null))
            {
                GameObject[] gameObjects = GetRelatedObjects(targets, target => (target as Component).gameObject);
                PopulateMenuForGameObject("", true, gameObjects);
            }
            else
            {
                PopulateMenuForObject(firstTarget.GetType().GetNameCS(BoolPref.ShowFullTypeNames) + " ->/", targets);
            }

            _Menu.AddSeparator("");

            BindingFlags bindings = BindingFlags.Static | BindingFlags.Public;
            if (BoolPref.ShowNonPublicMethods)
                bindings |= BindingFlags.NonPublic;

            PopulateMenuWithMembers(type, bindings, "", null);
        }

        /************************************************************************************************************************/

        private static void PopulateMenuForGameObject(string prefix, bool putGameObjectInSubMenu, Object[] targets)
        {
            GUIContent header = new GUIContent(prefix + "Selected GameObject and its Components");

            string gameObjectPrefix = prefix;
            if (putGameObjectInSubMenu)
            {
                _Menu.AddDisabledItem(header);
                gameObjectPrefix += "GameObject ->/";
            }

            PopulateMenuForObject(gameObjectPrefix, targets);

            if (!putGameObjectInSubMenu)
            {
                _Menu.AddSeparator(prefix);
                _Menu.AddDisabledItem(header);
            }

            GameObject[] gameObjects = GetRelatedObjects(targets, target => target as GameObject);
            PopulateMenuForComponents(prefix, gameObjects);
        }

        /************************************************************************************************************************/

        private static void PopulateMenuForComponents(string prefix, GameObject[] gameObjects)
        {
            GameObject firstGameObject = gameObjects[0];
            Component[] components = firstGameObject.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];

                Object[] targets = new Object[gameObjects.Length];
                targets[0] = component;

                Type type;
                int typeIndex = GetComponentTypeIndex(component, components, out type);

                int minTypeCount;
                Component unused;
                GetComponent(firstGameObject, type, typeIndex, out minTypeCount, out unused);

                int j = 1;
                for (; j < gameObjects.Length; j++)
                {
                    int typeCount;
                    Component targetComponent;
                    GetComponent(gameObjects[j], type, typeIndex, out typeCount, out targetComponent);
                    if (typeCount <= typeIndex)
                        goto NextComponent;

                    targets[j] = targetComponent;

                    if (minTypeCount > typeCount)
                        minTypeCount = typeCount;
                }

                string name = type.GetNameCS(BoolPref.ShowFullTypeNames) + " ->/";

                if (minTypeCount > 1)
                    name = UltEventUtils.GetPlacementName(typeIndex) + " " + name;

                PopulateMenuForObject(prefix + name, targets);
            }

            NextComponent:;
        }

        private static int GetComponentTypeIndex(Component component, Component[] components, out Type type)
        {
            type = component.GetType();

            int count = 0;

            for (int i = 0; i < components.Length; i++)
            {
                Component c = components[i];
                if (c == component)
                    break;
                if (c.GetType() == type)
                    count++;
            }

            return count;
        }

        private static void GetComponent(GameObject gameObject, Type type, int targetIndex, out int numberOfComponentsOfType, out Component targetComponent)
        {
            numberOfComponentsOfType = 0;
            targetComponent = null;

            Component[] components = gameObject.GetComponents(type);
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component.GetType() == type)
                {
                    if (numberOfComponentsOfType == targetIndex)
                        targetComponent = component;

                    numberOfComponentsOfType++;
                }
            }
        }

        /************************************************************************************************************************/

        private static void PopulateMenuForComponent(Object[] targets)
        {
            GameObject[] gameObjects = GetRelatedObjects(targets, target => (target as Component).gameObject);

            PopulateMenuForGameObject("", true, gameObjects);
            _Menu.AddSeparator("");

            PopulateMenuForObject(targets);
        }

        /************************************************************************************************************************/

        private static void PopulateMenuForObject(Object[] targets)
        {
            PopulateMenuForObject("", targets);
        }

        private static void PopulateMenuForObject(string prefix, Object[] targets)
        {
            PopulateMenuWithMembers(targets[0].GetType(), _Bindings, prefix, targets);
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Populate for Types
        /************************************************************************************************************************/

        private static void PopulateMenuWithMembers(Type type, BindingFlags bindings, string prefix, Object[] targets)
        {
            List<MemberInfo> members = GetSortedMembers(type, bindings);
            Type previousDeclaringType = type;

            bool firstSeparator = true;
            bool firstProperty = true;
            bool firstMethod = true;
            bool firstBaseType = true;
            bool nameMatchesNextMethod = false;

            int i = 0;
            while (i < members.Count)
            {
                ParameterInfo[] parameters;
                MethodInfo getter;
                MemberInfo member = GetNextSupportedMember(members, ref i, out parameters, out getter);

                GotMember:

                if (member == null)
                    return;

                i++;

                if (BoolPref.SubMenuForEachBaseType)
                {
                    if (firstBaseType && member.DeclaringType != type)
                    {
                        if (firstSeparator)
                            firstSeparator = false;
                        else
                            _Menu.AddSeparator(prefix);

                        string baseTypesOf = "Base Types of " + type.GetNameCS();
                        if (BoolPref.SubMenuForBaseTypes)
                        {
                            prefix += baseTypesOf + " ->/";
                        }
                        else
                        {
                            _Menu.AddDisabledItem(new GUIContent(prefix + baseTypesOf));
                        }
                        firstProperty = false;
                        firstMethod = false;
                        firstBaseType = false;
                    }

                    if (previousDeclaringType != member.DeclaringType)
                    {
                        previousDeclaringType = member.DeclaringType;
                        firstProperty = true;
                        firstMethod = true;
                        firstSeparator = true;
                    }
                }

                PropertyInfo property = member as PropertyInfo;
                if (property != null)
                {
                    AppendGroupHeader(prefix, "Properties in ", member.DeclaringType, type, ref firstProperty, ref firstSeparator);

                    AddSelectPropertyItem(prefix, targets, type, property, getter);
                    continue;
                }

                MethodBase method = member as MethodBase;
                if (method != null)
                {
                    AppendGroupHeader(prefix, "Methods in ", member.DeclaringType, type, ref firstMethod, ref firstSeparator);

                    // Check if the method name matched the previous or next method to group them.
                    if (BoolPref.GroupMethodOverloads)
                    {
                        bool nameMatchedPreviousMethod = nameMatchesNextMethod;

                        ParameterInfo[] nextParameters;
                        MethodInfo nextGetter;
                        MemberInfo nextMember = GetNextSupportedMember(members, ref i, out nextParameters, out nextGetter);

                        nameMatchesNextMethod = nextMember != null && method.Name == nextMember.Name;

                        if (nameMatchedPreviousMethod || nameMatchesNextMethod)
                        {
                            AddSelectMethodItem(prefix, targets, type, true, method, parameters);

                            if (i < members.Count)
                            {
                                member = nextMember;
                                parameters = nextParameters;
                                getter = nextGetter;
                                goto GotMember;
                            }

                            return;
                        }
                    }

                    // Otherwise just build the label normally.
                    AddSelectMethodItem(prefix, targets, type, false, method, parameters);
                }
            }
        }

        /************************************************************************************************************************/

        private static void AppendGroupHeader(string prefix, string name, Type declaringType, Type currentType, ref bool firstInGroup, ref bool firstSeparator)
        {
            if (firstInGroup)
            {
                LabelBuilder.Length = 0;
                LabelBuilder.Append(prefix);

                if (BoolPref.SubMenuForEachBaseType && declaringType != currentType)
                    AppendDeclaringTypeSubMenu(LabelBuilder, declaringType, currentType);

                if (firstSeparator)
                    firstSeparator = false;
                else
                    _Menu.AddSeparator(LabelBuilder.ToString());

                LabelBuilder.Append(name);

                if (BoolPref.SubMenuForEachBaseType)
                    LabelBuilder.Append(declaringType.GetNameCS());
                else
                    LabelBuilder.Append(currentType.GetNameCS());

                _Menu.AddDisabledItem(new GUIContent(LabelBuilder.ToString()));
                firstInGroup = false;
            }
        }

        private static void AppendDeclaringTypeSubMenu(StringBuilder text, Type declaringType, Type currentType)
        {
            if (BoolPref.SubMenuForEachBaseType)
            {
                if (BoolPref.SubMenuForRootBaseType || declaringType != currentType)
                {
                    text.Append(declaringType.GetNameCS());
                    text.Append(" ->/");
                }
            }
        }

        /************************************************************************************************************************/

        private static void AddSelectPropertyItem(string prefix, Object[] targets, Type currentType, PropertyInfo property, MethodInfo getter)
        {
            MethodInfo defaultMethod = getter;

            MethodInfo setter = null;
            if (IsSupported(property.PropertyType))
            {
                setter = property.GetSetMethod(true);
                if (setter != null)
                    defaultMethod = setter;
            }

            LabelBuilder.Length = 0;
            LabelBuilder.Append(prefix);

            // Declaring Type.
            AppendDeclaringTypeSubMenu(LabelBuilder, property.DeclaringType, currentType);

            // Non-Public Grouping.
            if (BoolPref.GroupNonPublicMethods && !IsPublic(property))
                LabelBuilder.Append("Non-Public Properties ->/");

            // Property Type and Name.
            LabelBuilder.Append(property.PropertyType.GetNameCS(BoolPref.ShowFullTypeNames));
            LabelBuilder.Append(' ');
            LabelBuilder.Append(property.Name);

            // Get and Set.
            LabelBuilder.Append(" { ");
            if (getter != null) LabelBuilder.Append("get; ");
            if (setter != null) LabelBuilder.Append("set; ");
            LabelBuilder.Append('}');

            string label = LabelBuilder.ToString();
            AddSetCallItem(label, defaultMethod, targets);
        }

        /************************************************************************************************************************/

        private static void AddSelectMethodItem(string prefix, Object[] targets, Type currentType, bool methodNameSubMenu,
            MethodBase method, ParameterInfo[] parameters)
        {
            LabelBuilder.Length = 0;
            LabelBuilder.Append(prefix);

            // Declaring Type.
            AppendDeclaringTypeSubMenu(LabelBuilder, method.DeclaringType, currentType);

            // Non-Public Grouping.
            if (BoolPref.GroupNonPublicMethods && !IsPublic(method))
                LabelBuilder.Append("Non-Public Methods ->/");

            // Overload Grouping.
            if (methodNameSubMenu)
                LabelBuilder.Append(method.Name).Append(" ->/");

            // Method Signature.
            LabelBuilder.Append(GetMethodSignature(method, parameters, true));

            string label = LabelBuilder.ToString();

            AddSetCallItem(label, method, targets);
        }

        /************************************************************************************************************************/

        private static void AddSetCallItem(string label, MethodBase method, Object[] targets)
        {
            _Menu.AddItem(
                new GUIContent(label),
                method == _CurrentMethod,
                userData =>
                {
                    DrawerState.Current.CopyFrom(CachedState);

                    int i = 0;
                    SerializedPropertyAccessor.ModifyValues<PersistentCall>(CachedState.CallProperty, call =>
                    {
                        Object target = targets != null ? targets[i % targets.Length] : null;
                        call.SetMethod(method, target);
                        i++;
                    }, "Set Persistent Call");

                    DrawerState.Current.Clear();
                },
                null);
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Member Gathering
        /************************************************************************************************************************/

        private static readonly Dictionary<BindingFlags, Dictionary<Type, List<MemberInfo>>>
            MemberCache = new Dictionary<BindingFlags, Dictionary<Type, List<MemberInfo>>>();

        internal static void ClearMemberCache()
        {
            MemberCache.Clear();
        }

        /************************************************************************************************************************/

        private static List<MemberInfo> GetSortedMembers(Type type, BindingFlags bindings)
        {
            // Get the cache for the specified bindings.
            Dictionary<Type, List<MemberInfo>> memberCache;
            if (!MemberCache.TryGetValue(bindings, out memberCache))
            {
                memberCache = new Dictionary<Type, List<MemberInfo>>();
                MemberCache.Add(bindings, memberCache);
            }

            // If the members for the specified type aren't cached for those bindings, gather and sort them.
            List<MemberInfo> members;
            if (!memberCache.TryGetValue(type, out members))
            {
                PropertyInfo[] properties = type.GetProperties(bindings);
                MethodInfo[] methods = type.GetMethods(bindings);

                // When gathering static members, also include instance constructors.
                ConstructorInfo[] constructors = ((bindings & BindingFlags.Static) == BindingFlags.Static) ?
                                                     type.GetConstructors((bindings & ~BindingFlags.Static) | BindingFlags.Instance) :
                                                     null;

                int capacity = properties.Length + methods.Length;
                if (constructors != null)
                    capacity += constructors.Length;

                members = new List<MemberInfo>(capacity);
                members.AddRange(properties);
                if (constructors != null)
                    members.AddRange(constructors);
                members.AddRange(methods);

                // If the bindings include static, add static members from each base type.
                if ((bindings & BindingFlags.Static) == BindingFlags.Static && type.BaseType != null)
                {
                    members.AddRange(GetSortedMembers(type.BaseType, bindings & ~BindingFlags.Instance));
                }

                UltEventUtils.StableInsertionSort(members, CompareMembers);

                memberCache.Add(type, members);
            }

            return members;
        }

        /************************************************************************************************************************/

        private static int CompareMembers(MemberInfo a, MemberInfo b)
        {
            if (BoolPref.SubMenuForEachBaseType)
            {
                int result = CompareChildBeforeBase(a.DeclaringType, b.DeclaringType);
                if (result != 0)
                    return result;
            }

            // Compare types (properties before methods).
            if (a is PropertyInfo)
            {
                if (!(b is PropertyInfo))
                    return -1;
            }
            else
            {
                if (b is PropertyInfo)
                    return 1;
            }

            // Non-Public Sub-Menu.
            if (BoolPref.GroupNonPublicMethods)
            {
                if (IsPublic(a))
                {
                    if (!IsPublic(b))
                        return -1;
                }
                else
                {
                    if (IsPublic(b))
                        return 1;
                }
            }

            // Compare names.
            return a.Name.CompareTo(b.Name);
        }

        /************************************************************************************************************************/

        private static int CompareChildBeforeBase(Type a, Type b)
        {
            if (a == b)
                return 0;

            while (true)
            {
                a = a.BaseType;

                if (a == null)
                    return 1;

                if (a == b)
                    return -1;
            }
        }

        /************************************************************************************************************************/

        private static readonly Dictionary<MemberInfo, bool>
            MemberToIsPublic = new Dictionary<MemberInfo, bool>();

        private static bool IsPublic(MemberInfo member)
        {
            bool isPublic;
            if (!MemberToIsPublic.TryGetValue(member, out isPublic))
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Constructor:
                    case MemberTypes.Method:
                        isPublic = (member as MethodBase).IsPublic;
                        break;

                    case MemberTypes.Property:
                        isPublic =
                            (member as PropertyInfo).GetGetMethod() != null ||
                            (member as PropertyInfo).GetSetMethod() != null;
                        break;

                    default:
                        throw new ArgumentException("Unhandled member type", "member");
                }

                MemberToIsPublic.Add(member, isPublic);
            }

            return isPublic;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Supported Checks
        /************************************************************************************************************************/

        private static bool IsSupported(MethodBase method, out ParameterInfo[] parameters)
        {
            if (method.IsGenericMethod ||
                (method.IsSpecialName && (!method.IsConstructor || method.IsStatic)) ||
                method.Name.Contains("<") ||
                method.IsDefined(typeof(ObsoleteAttribute), true))
            {
                parameters = null;
                return false;
            }

            // Most UnityEngine.Object types shouldn't be constructed directly.
            if (method.IsConstructor)
            {
                if (typeof(Component).IsAssignableFrom(method.DeclaringType) ||
                    typeof(ScriptableObject).IsAssignableFrom(method.DeclaringType))
                {
                    parameters = null;
                    return false;
                }
            }

            parameters = method.GetParameters();
            if (!IsSupported(parameters))
                return false;

            return true;
        }

        private static bool IsSupported(PropertyInfo property, out MethodInfo getter)
        {
            if (property.IsSpecialName ||
                property.IsDefined(typeof(ObsoleteAttribute), true))// Obsolete.
            {
                getter = null;
                return false;
            }

            getter = property.GetGetMethod(true);
            if (getter == null && !IsSupported(property.PropertyType))
                return false;

            return true;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns true if the specified 'type' can be represented by a <see cref="PersistentArgument"/>.
        /// </summary>
        public static bool IsSupported(Type type)
        {
            if (PersistentCall.IsSupportedNative(type))
            {
                return true;
            }

            int                    linkIndex;
            PersistentArgumentType linkType;
            return DrawerState.Current.TryGetLinkable(type, out linkIndex, out linkType);
        }

        /// <summary>
        /// Returns true if the type of each of the 'parameters' can be represented by a <see cref="PersistentArgument"/>.
        /// </summary>
        public static bool IsSupported(ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (!IsSupported(parameters[i].ParameterType))
                    return false;
            }

            return true;
        }

        /************************************************************************************************************************/

        private static MemberInfo GetNextSupportedMember(List<MemberInfo> members, ref int startIndex, out ParameterInfo[] parameters, out MethodInfo getter)
        {
            while (startIndex < members.Count)
            {
                MemberInfo member = members[startIndex];
                PropertyInfo property = member as PropertyInfo;
                if (property != null && IsSupported(property, out getter))
                {
                    parameters = null;
                    return member;
                }

                MethodBase method = member as MethodBase;
                if (method != null && IsSupported(method, out parameters))
                {
                    getter = null;
                    return member;
                }

                startIndex++;
            }

            parameters = null;
            getter = null;
            return null;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Method Signatures
        /************************************************************************************************************************/

        private static readonly Dictionary<MethodBase, string>
            MethodSignaturesWithParameters = new Dictionary<MethodBase, string>(),
            MethodSignaturesWithoutParameters = new Dictionary<MethodBase, string>();
        private static readonly StringBuilder
            MethodSignatureBuilder = new StringBuilder();

        /************************************************************************************************************************/

        public static string GetMethodSignature(MethodBase method, ParameterInfo[] parameters, bool includeParameterNames)
        {
            if (method == null)
                return null;

            Dictionary<MethodBase, string> signatureCache = includeParameterNames ? MethodSignaturesWithParameters : MethodSignaturesWithoutParameters;

            string signature;
            if (!signatureCache.TryGetValue(method, out signature))
            {
                signature = BuildAndCacheSignature(method, parameters, includeParameterNames, signatureCache);
            }

            return signature;
        }

        public static string GetMethodSignature(MethodBase method, bool includeParameterNames)
        {
            if (method == null)
                return null;

            Dictionary<MethodBase, string> signatureCache = includeParameterNames ? MethodSignaturesWithParameters : MethodSignaturesWithoutParameters;

            string signature;
            if (!signatureCache.TryGetValue(method, out signature))
            {
                signature = BuildAndCacheSignature(method, method.GetParameters(), includeParameterNames, signatureCache);
            }

            return signature;
        }

        /************************************************************************************************************************/

        private static string BuildAndCacheSignature(MethodBase method, ParameterInfo[] parameters, bool includeParameterNames,
            Dictionary<MethodBase, string> signatureCache)
        {
            MethodSignatureBuilder.Length = 0;

            Type returnType = method.GetReturnType();
            MethodSignatureBuilder.Append(returnType.GetNameCS(false));
            MethodSignatureBuilder.Append(' ');

            MethodSignatureBuilder.Append(method.Name);

            MethodSignatureBuilder.Append(" (");
            for (int i = 0; i < parameters.Length; i++)
            {
                if (i > 0)
                    MethodSignatureBuilder.Append(", ");

                ParameterInfo parameter = parameters[i];

                MethodSignatureBuilder.Append(parameter.ParameterType.GetNameCS(false));
                if (includeParameterNames)
                {
                    MethodSignatureBuilder.Append(' ');
                    MethodSignatureBuilder.Append(parameter.Name);
                }
            }
            MethodSignatureBuilder.Append(')');

            string signature = MethodSignatureBuilder.ToString();
            MethodSignatureBuilder.Length = 0;
            signatureCache.Add(method, signature);

            return signature;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif