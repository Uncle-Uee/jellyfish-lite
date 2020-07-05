// Created by Kearan Petersen : https://www.blumalice.wordpress.com | https://www.linkedin.com/in/kearan-petersen/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SOFlow.Fading
{
    public class FadableGroup : Fadable
    {
        /// <summary>
        ///     The list of fadables managed by his fadable group.
        /// </summary>
        public List<Fadable> Fadables = new List<Fadable>();

        /// <inheritdoc />
        protected override Color GetColour()
        {
            return default;
        }

        /// <inheritdoc />
        public override void UpdateColour(Color colour, float percentage)
        {
            foreach (Fadable fadable in Fadables) fadable.OnUpdateColour(colour, percentage);
        }

        /// <summary>
        ///     Captures all child fadables.
        /// </summary>
        public void CaptureChildFadables()
        {
            Fadables.Clear();

            List<Fadable> fadables = GetComponentsInChildren<Fadable>().ToList();
            fadables.Remove(this);

            Fadables.AddRange(fadables);
        }
    }
}