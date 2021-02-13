using System.Collections;
using System.Reflection;
using UnityEngine;
using System;
using System.Linq;

namespace SoccerMasterz.Utils
{
    internal class DelayedJobExecutor
    {
        public static DelayedJobExecutor CreateJob(MonoBehaviour monoBehaviour) =>
            new DelayedJobExecutor(monoBehaviour);

        private readonly MonoBehaviour monoBehaviour;

        private DelayedJobExecutor(MonoBehaviour monoBehaviour) => this.monoBehaviour = monoBehaviour;

        internal JobDescription DelayExecute(float delayInSeconds, Action lambda) =>
            new JobDescription(monoBehaviour.StartCoroutine(Delayed(delayInSeconds, lambda)));

        internal JobDescription DelayExecute(float delayInSeconds, Action<object[]> lambda, params object[] parameters) =>
            new JobDescription(monoBehaviour.StartCoroutine(Delayed(delayInSeconds, lambda, parameters)));

        public JobDescription DelayExecute(float delayInSeconds, string methodName, params object[] parameters) =>
            (from method in monoBehaviour.GetType().GetMethods()
                where method.Name.Equals(methodName)
                select new JobDescription(monoBehaviour.StartCoroutine(Delayed(delayInSeconds, method, parameters))))
            .FirstOrDefault();

        internal void StopExecution(JobDescription id) =>
            monoBehaviour.StopCoroutine(id.coroutine);

        private IEnumerator Delayed(float delayInSeconds, Action lambda)
        {
            yield return new WaitForSeconds(delayInSeconds);
            lambda.Invoke();
        }

        private IEnumerator Delayed(float delayInSeconds, Action<object[]> lambda, params object[] parameters)
        {
            yield return new WaitForSeconds(delayInSeconds);
            lambda.Invoke(parameters);
        }

        private IEnumerator Delayed(float delayInSeconds, MethodInfo method, params object[] parameters)
        {
            yield return new WaitForSeconds(delayInSeconds);
            method.Invoke(monoBehaviour, parameters);
        }

        private IEnumerator Delayed(Func<bool> condition, Action<object[]> lambda, params object[] parameters)
        {
            yield return new WaitUntil(condition);
            lambda.Invoke(parameters);
        }

        private IEnumerator Delayed(Func<bool> condition, MethodInfo method, params object[] parameters)
        {
            yield return new WaitUntil(condition);
            method.Invoke(monoBehaviour, parameters);
        }

        internal class JobDescription
        {
            internal readonly Coroutine coroutine;
            internal JobDescription(Coroutine coroutine) => this.coroutine = coroutine;
        }
    }
}