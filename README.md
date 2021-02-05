<!-- omit in toc -->
# PlantAI

<!-- omit in toc -->
## Table of Contents

- [Context](#context)
- [What is it?](#what-is-it)
  - [Introduction](#introduction)
  - [Dynamic animation](#dynamic-animation)
  - [Lighting behaviour](#lighting-behaviour)
    - [Effect on leaves](#effect-on-leaves)
  - [Environment](#environment)
- [Contributors](#contributors)

## Context

**PlantAI** is a simple project made with Unity (2019.4.17f1) in the context of an IMAC third year course.
The purpose of this course was to design some Artificial Intelligence in a 3D way.

## What is it?

### Introduction

**PlantAI** is a kind of FPS game which takes place in a greenhouse and where we can interact with the environment. Doing so, we can allow (or not) the plants to grow.

As we all know, plants need light and space because they dynamically behave with their environment to become bigger and greater. The player can experiment like this to see the extent to which they can live.

### Dynamic animation

To make the plants grow dynamically in real-time, we decided to use *ProBuilder* inside of Unity.

The basic idea is quite simple: we start with a cylinder and we extrude one of its faces through time to give the illusion of growing. We also create some children branches to create a real natural object which expands over space.

Like most plants, they tend to grow up to the sky. One of the limitations of our implementation is we do not take in count the gravity parameter.

:pencil: To avoid very long branch (more details in [Lighting behaviour](#lighting-behaviour)), we create a new branch in the continuity of the first one. Like this, it looks like a unique branch but actually, it is not.

### Lighting behaviour

Because plants need light to live, we made a system that allow us to know exactly how much light a branch receives. If it receives an enough quantity, it can grow a bit. If not, it just stops (and starts to die).

The system uses a camera rig that takes a lot of pictures around each object, and compute the data to determine how much the object is illuminated. The system runs in another thread, so it does not affect the general performances.
Because of doing this, the branch cannot be as long as possible to avoid the camera taking pictures to go beyond its sensor capacities.

#### Effect on leaves

When the plant grows, some leaves born on its surface. But the leaves preferred to be oriented to the light, so they tend to born on one of the most illuminated side of the branch.

### Environment

We decided to make the game inside of a greenhouse because it is quite interesting to see how plants can "work" together and live to create a beautiful natural set.

They have the ability to avoid obstacles. Unfortunately, they are not able to wrap an object and act like real ivy, for instance.

## Contributors

- Ruben BRAMI
- Julien HAUDEGOND
- Antoine LIBERT
- Flora MALLET
