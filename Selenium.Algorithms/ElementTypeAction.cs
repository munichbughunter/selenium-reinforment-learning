﻿namespace Selenium.Algorithms
{
    using OpenQA.Selenium;
    using Selenium.Algorithms.ReinforcementLearning;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ElementTypeAction : AgentAction<IReadOnlyCollection<ElementData>>
    {
        private readonly ElementData webElement;
        private readonly string text;
        public readonly string CachedName;
        public readonly int CachedHash;

        public ElementTypeAction(in ElementData webElement, in string text)
        {
            this.webElement = webElement;
            this.text = text;

            // We have to cache those values because the element will get out of the DOM eventually
            CachedHash = webElement.ExtendedGetHashCode();
            CachedHash = (CachedHash * 7) + text.GetHashCode();
            CachedName = $"Type '{text}' on {webElement.ExtendedToString()}";
        }

        public override async Task<State<IReadOnlyCollection<ElementData>>> ExecuteAction(Environment<IReadOnlyCollection<ElementData>> environment, State<IReadOnlyCollection<ElementData>> state)
        {
            try
            {
                Console.Write($"\t- typing on {CachedName}");
                webElement.WebElementReference.Clear();
                webElement.WebElementReference.SendKeys(text);
                Console.WriteLine($" ... done!");
            }
            catch (ElementNotInteractableException)
            {
                Console.WriteLine($"... failed: non-interactable");
                // Do not move at all if we cannot click, it should penalize it
            }
            catch (StaleElementReferenceException)
            {
                Console.WriteLine($"... failed: stale");
                // Do not move at all if we cannot click, it should penalize it
            }

            return (environment as SeleniumEnvironment).GetCurrentState();
        }

        public override bool Equals(object obj)
        {
            return obj is ElementTypeAction otherAction
                && CachedHash == otherAction.CachedHash;
        }

        public override int GetHashCode()
        {
            return CachedHash;
        }

        public override string ToString()
        {
            return CachedName;
        }
    }
}