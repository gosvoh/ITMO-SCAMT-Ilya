// Copyright (c) Intangible Realities Lab. All rights reserved.
// Licensed under the GPL. See License.txt in the project root for license information.

using Narupa.Frame.Event;
using NUnit.Framework;

namespace Narupa.Frame.Tests
{
    internal class FrameChangesTests
    {
        [Test]
        public void Initial_HasAnythingChanged()
        {
            var changes = new FrameChanges();
            Assert.IsFalse(changes.HasAnythingChanged);
        }

        [Test]
        public void Initial_HasRandomChanged()
        {
            var changes = new FrameChanges();
            Assert.IsFalse(changes.GetIsChanged("id"));
        }

        [Test]
        public void SetIsChanged()
        {
            var changes = new FrameChanges();
            changes.SetIsChanged("id", true);
            Assert.IsTrue(changes.GetIsChanged("id"));
        }

        [Test]
        public void SetIsChanged_HasAnythingChanged()
        {
            var changes = new FrameChanges();
            changes.SetIsChanged("id", true);
            Assert.IsTrue(changes.HasAnythingChanged);
        }

        [Test]
        public void SetThenUnsetIsChanged()
        {
            var changes = new FrameChanges();
            changes.SetIsChanged("id", true);
            changes.SetIsChanged("id", false);
            Assert.IsFalse(changes.GetIsChanged("id"));
        }

        [Test]
        public void SetThenUnsetIsChanged_HasAnythingChanged()
        {
            var changes = new FrameChanges();
            changes.SetIsChanged("id", true);
            changes.SetIsChanged("id", false);
            Assert.IsFalse(changes.HasAnythingChanged);
        }

        [Test]
        public void Initial_HaveBondsChanged()
        {
            var changes = new FrameChanges();
            Assert.IsFalse(changes.HaveBondsChanged);
        }

        [Test]
        public void Initial_HavePositionsChanged()
        {
            var changes = new FrameChanges();
            Assert.IsFalse(changes.HaveParticlePositionsChanged);
        }

        [Test]
        public void Initial_HaveElementsChanged()
        {
            var changes = new FrameChanges();
            Assert.IsFalse(changes.HaveParticleElementsChanged);
        }

        [Test]
        public void SetHaveBondsChanged()
        {
            var changes = new FrameChanges();
            changes.HaveBondsChanged = true;
            Assert.IsTrue(changes.HaveBondsChanged);
        }

        [Test]
        public void SetHavePositionsChanged()
        {
            var changes = new FrameChanges();
            changes.HaveParticlePositionsChanged = true;
            Assert.IsTrue(changes.HaveParticlePositionsChanged);
        }

        [Test]
        public void SetHaveElementsChanged()
        {
            var changes = new FrameChanges();
            changes.HaveParticleElementsChanged = true;
            Assert.IsTrue(changes.HaveParticleElementsChanged);
        }

        [Test]
        public void Merge_ChangesWithEmpty()
        {
            var original = new FrameChanges();
            var next = new FrameChanges();
            next.SetIsChanged("id", true);
            original.MergeChanges(next);
            Assert.IsTrue(original.GetIsChanged("id"));
        }

        [Test]
        public void Merge_EmptyWithChanges()
        {
            var original = new FrameChanges();
            original.SetIsChanged("id", true);

            var next = new FrameChanges();

            original.MergeChanges(next);
            Assert.IsTrue(original.GetIsChanged("id"));
        }
    }
}