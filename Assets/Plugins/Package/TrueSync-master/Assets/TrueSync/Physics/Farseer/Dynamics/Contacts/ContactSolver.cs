/*
* Farseer Physics Engine:
* Copyright (c) 2012 Ian Qvist
* 
* Original source Box2D:
* Copyright (c) 2006-2011 Erin Catto http://www.box2d.org 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/
#pragma warning disable 0162

using System.Diagnostics;

namespace TrueSync.Physics2D
{
    public sealed class ContactPositionConstraint
    {
        public TSVector2[] localPoints = new TSVector2[Settings.MaxManifoldPoints];
        public TSVector2 localNormal;
        public TSVector2 localPoint;
        public int indexA;
        public int indexB;
        public FP invMassA, invMassB;
        public TSVector2 localCenterA, localCenterB;
        public FP invIA, invIB;
        public ManifoldType type;
        public FP radiusA, radiusB;
        public int pointCount;
    }

    public sealed class VelocityConstraintPoint
    {
        public TSVector2 rA;
        public TSVector2 rB;
        public FP normalImpulse;
        public FP tangentImpulse;
        public FP normalMass;
        public FP tangentMass;
        public FP velocityBias;
    }

    public sealed class ContactVelocityConstraint
    {
        public VelocityConstraintPoint[] points = new VelocityConstraintPoint[Settings.MaxManifoldPoints];
        public TSVector2 normal;
        public Mat22 normalMass;
        public Mat22 K;
        public int indexA;
        public int indexB;
        public FP invMassA, invMassB;
        public FP invIA, invIB;
        public FP friction;
        public FP restitution;
        public FP tangentSpeed;
        public int pointCount;
        public int contactIndex;

        public ContactVelocityConstraint()
        {
            for (int i = 0; i < Settings.MaxManifoldPoints; i++)
            {
                points[i] = new VelocityConstraintPoint();
            }
        }
    }

    public class ContactSolver
    {
        public TimeStep _step;
        public Position[] _positions;
        public Velocity[] _velocities;
        public ContactPositionConstraint[] _positionConstraints;
        public ContactVelocityConstraint[] _velocityConstraints;
        public Contact[] _contacts;
        public int _count;

        public void Reset(TimeStep step, int count, Contact[] contacts, Position[] positions, Velocity[] velocities, bool warmstarting = Settings.EnableWarmstarting)
        {
            _step = step;
            _count = count;
            _positions = positions;
            _velocities = velocities;
            _contacts = contacts;

            // grow the array
            if (_velocityConstraints == null || _velocityConstraints.Length < count)
            {
                _velocityConstraints = new ContactVelocityConstraint[count * 2];
                _positionConstraints = new ContactPositionConstraint[count * 2];

                for (int i = 0; i < _velocityConstraints.Length; i++)
                {
                    _velocityConstraints[i] = new ContactVelocityConstraint();
                }

                for (int i = 0; i < _positionConstraints.Length; i++)
                {
                    _positionConstraints[i] = new ContactPositionConstraint();
                }
            }

            // Initialize position independent portions of the constraints.
            for (int i = 0; i < _count; ++i)
            {
                Contact contact = contacts[i];

                Fixture fixtureA = contact.FixtureA;
                Fixture fixtureB = contact.FixtureB;
                Shape shapeA = fixtureA.Shape;
                Shape shapeB = fixtureB.Shape;
                FP radiusA = shapeA.Radius;
                FP radiusB = shapeB.Radius;
                Body bodyA = fixtureA.Body;
                Body bodyB = fixtureB.Body;
                Manifold manifold = contact.Manifold;

                int pointCount = manifold.PointCount;
                Debug.Assert(pointCount > 0);

                ContactVelocityConstraint vc = _velocityConstraints[i];
                vc.friction = contact.Friction;
                vc.restitution = contact.Restitution;
                vc.tangentSpeed = contact.TangentSpeed;
                vc.indexA = bodyA.IslandIndex;
                vc.indexB = bodyB.IslandIndex;
                vc.invMassA = bodyA._invMass;
                vc.invMassB = bodyB._invMass;
                vc.invIA = bodyA._invI;
                vc.invIB = bodyB._invI;
                vc.contactIndex = i;
                vc.pointCount = pointCount;
                vc.K.SetZero();
                vc.normalMass.SetZero();

                ContactPositionConstraint pc = _positionConstraints[i];
                pc.indexA = bodyA.IslandIndex;
                pc.indexB = bodyB.IslandIndex;
                pc.invMassA = bodyA._invMass;
                pc.invMassB = bodyB._invMass;
                pc.localCenterA = bodyA._sweep.LocalCenter;
                pc.localCenterB = bodyB._sweep.LocalCenter;
                pc.invIA = bodyA._invI;
                pc.invIB = bodyB._invI;
                pc.localNormal = manifold.LocalNormal;
                pc.localPoint = manifold.LocalPoint;
                pc.pointCount = pointCount;
                pc.radiusA = radiusA;
                pc.radiusB = radiusB;
                pc.type = manifold.Type;

                for (int j = 0; j < pointCount; ++j)
                {
                    ManifoldPoint cp = manifold.Points[j];
                    VelocityConstraintPoint vcp = vc.points[j];

                    if (Settings.EnableWarmstarting)
                    {
                        vcp.normalImpulse = _step.dtRatio * cp.NormalImpulse;
                        vcp.tangentImpulse = _step.dtRatio * cp.TangentImpulse;
                    }
                    else
                    {
                        vcp.normalImpulse = 0.0f;
                        vcp.tangentImpulse = 0.0f;
                    }

                    vcp.rA = TSVector2.zero;
                    vcp.rB = TSVector2.zero;
                    vcp.normalMass = 0.0f;
                    vcp.tangentMass = 0.0f;
                    vcp.velocityBias = 0.0f;

                    pc.localPoints[j] = cp.LocalPoint;
                }
            }
        }

        public void InitializeVelocityConstraints()
        {
            for (int i = 0; i < _count; ++i)
            {
                ContactVelocityConstraint vc = _velocityConstraints[i];
                ContactPositionConstraint pc = _positionConstraints[i];

                FP radiusA = pc.radiusA;
                FP radiusB = pc.radiusB;
                Manifold manifold = _contacts[vc.contactIndex].Manifold;

                int indexA = vc.indexA;
                int indexB = vc.indexB;

                FP mA = vc.invMassA;
                FP mB = vc.invMassB;
                FP iA = vc.invIA;
                FP iB = vc.invIB;
                TSVector2 localCenterA = pc.localCenterA;
                TSVector2 localCenterB = pc.localCenterB;

                TSVector2 cA = _positions[indexA].c;
                FP aA = _positions[indexA].a;
                TSVector2 vA = _velocities[indexA].v;
                FP wA = _velocities[indexA].w;

                TSVector2 cB = _positions[indexB].c;
                FP aB = _positions[indexB].a;
                TSVector2 vB = _velocities[indexB].v;
                FP wB = _velocities[indexB].w;

                Debug.Assert(manifold.PointCount > 0);

                Transform xfA = new Transform();
                Transform xfB = new Transform();
                xfA.q.Set(aA);
                xfB.q.Set(aB);
                xfA.p = cA - MathUtils.Mul(xfA.q, localCenterA);
                xfB.p = cB - MathUtils.Mul(xfB.q, localCenterB);

                TSVector2 normal;
                FixedArray2<TSVector2> points;
                WorldManifold.Initialize(ref manifold, ref xfA, radiusA, ref xfB, radiusB, out normal, out points);

                vc.normal = normal;

                int pointCount = vc.pointCount;
                for (int j = 0; j < pointCount; ++j)
                {
                    VelocityConstraintPoint vcp = vc.points[j];

                    vcp.rA = points[j] - cA;
                    vcp.rB = points[j] - cB;

                    FP rnA = MathUtils.Cross(vcp.rA, vc.normal);
                    FP rnB = MathUtils.Cross(vcp.rB, vc.normal);

                    FP kNormal = mA + mB + iA * rnA * rnA + iB * rnB * rnB;

                    vcp.normalMass = kNormal > 0.0f ? 1.0f / kNormal : 0.0f;

                    TSVector2 tangent = MathUtils.Cross(vc.normal, 1.0f);

                    FP rtA = MathUtils.Cross(vcp.rA, tangent);
                    FP rtB = MathUtils.Cross(vcp.rB, tangent);

                    FP kTangent = mA + mB + iA * rtA * rtA + iB * rtB * rtB;

                    vcp.tangentMass = kTangent > 0.0f ? 1.0f / kTangent : 0.0f;

                    // Setup a velocity bias for restitution.
                    vcp.velocityBias = 0.0f;
                    FP vRel = TSVector2.Dot(vc.normal, vB + MathUtils.Cross(wB, vcp.rB) - vA - MathUtils.Cross(wA, vcp.rA));
                    if (vRel < -Settings.VelocityThreshold)
                    {
                        vcp.velocityBias = -vc.restitution * vRel;
                    }
                }

                // If we have two points, then prepare the block solver.
                if (vc.pointCount == 2)
                {
                    VelocityConstraintPoint vcp1 = vc.points[0];
                    VelocityConstraintPoint vcp2 = vc.points[1];

                    FP rn1A = MathUtils.Cross(vcp1.rA, vc.normal);
                    FP rn1B = MathUtils.Cross(vcp1.rB, vc.normal);
                    FP rn2A = MathUtils.Cross(vcp2.rA, vc.normal);
                    FP rn2B = MathUtils.Cross(vcp2.rB, vc.normal);

                    FP k11 = mA + mB + iA * rn1A * rn1A + iB * rn1B * rn1B;
                    FP k22 = mA + mB + iA * rn2A * rn2A + iB * rn2B * rn2B;
                    FP k12 = mA + mB + iA * rn1A * rn2A + iB * rn1B * rn2B;

                    // Ensure a reasonable condition number.
                    FP k_maxConditionNumber = 1000.0f;
                    if (k11 * k11 < k_maxConditionNumber * (k11 * k22 - k12 * k12))
                    {
                        // K is safe to invert.
                        vc.K.ex = new TSVector2(k11, k12);
                        vc.K.ey = new TSVector2(k12, k22);
                        vc.normalMass = vc.K.Inverse;
                    }
                    else
                    {
                        // The constraints are redundant, just use one.
                        // TODO_ERIN use deepest?
                        vc.pointCount = 1;
                    }
                }
            }
        }

        public void WarmStart()
        {
            // Warm start.
            for (int i = 0; i < _count; ++i)
            {
                ContactVelocityConstraint vc = _velocityConstraints[i];

                int indexA = vc.indexA;
                int indexB = vc.indexB;
                FP mA = vc.invMassA;
                FP iA = vc.invIA;
                FP mB = vc.invMassB;
                FP iB = vc.invIB;
                int pointCount = vc.pointCount;

                TSVector2 vA = _velocities[indexA].v;
                FP wA = _velocities[indexA].w;
                TSVector2 vB = _velocities[indexB].v;
                FP wB = _velocities[indexB].w;

                TSVector2 normal = vc.normal;
                TSVector2 tangent = MathUtils.Cross(normal, 1.0f);

                for (int j = 0; j < pointCount; ++j)
                {
                    VelocityConstraintPoint vcp = vc.points[j];
                    TSVector2 P = vcp.normalImpulse * normal + vcp.tangentImpulse * tangent;
                    wA -= iA * MathUtils.Cross(vcp.rA, P);
                    vA -= mA * P;
                    wB += iB * MathUtils.Cross(vcp.rB, P);
                    vB += mB * P;
                }

                _velocities[indexA].v = vA;
                _velocities[indexA].w = wA;
                _velocities[indexB].v = vB;
                _velocities[indexB].w = wB;
            }
        }

        public void SolveVelocityConstraints()
        {
            for (int i = 0; i < _count; ++i)
            {
                ContactVelocityConstraint vc = _velocityConstraints[i];

                int indexA = vc.indexA;
                int indexB = vc.indexB;
                FP mA = vc.invMassA;
                FP iA = vc.invIA;
                FP mB = vc.invMassB;
                FP iB = vc.invIB;
                int pointCount = vc.pointCount;

                TSVector2 vA = _velocities[indexA].v;
                FP wA = _velocities[indexA].w;
                TSVector2 vB = _velocities[indexB].v;
                FP wB = _velocities[indexB].w;

                TSVector2 normal = vc.normal;
                TSVector2 tangent = MathUtils.Cross(normal, 1.0f);
                FP friction = vc.friction;

                Debug.Assert(pointCount == 1 || pointCount == 2);

                // Solve tangent constraints first because non-penetration is more important
                // than friction.
                for (int j = 0; j < pointCount; ++j)
                {
                    VelocityConstraintPoint vcp = vc.points[j];

                    // Relative velocity at contact
                    TSVector2 dv = vB + MathUtils.Cross(wB, vcp.rB) - vA - MathUtils.Cross(wA, vcp.rA);

                    // Compute tangent force
                    FP vt = TSVector2.Dot(dv, tangent) - vc.tangentSpeed;
                    FP lambda = vcp.tangentMass * (-vt);

                    // b2Clamp the accumulated force
                    FP maxFriction = friction * vcp.normalImpulse;
                    FP newImpulse = MathUtils.Clamp(vcp.tangentImpulse + lambda, -maxFriction, maxFriction);
                    lambda = newImpulse - vcp.tangentImpulse;
                    vcp.tangentImpulse = newImpulse;

                    // Apply contact impulse
                    TSVector2 P = lambda * tangent;

                    vA -= mA * P;
                    wA -= iA * MathUtils.Cross(vcp.rA, P);

                    vB += mB * P;
                    wB += iB * MathUtils.Cross(vcp.rB, P);
                }

                // Solve normal constraints
                if (vc.pointCount == 1)
                {
                    VelocityConstraintPoint vcp = vc.points[0];

                    // Relative velocity at contact
                    TSVector2 dv = vB + MathUtils.Cross(wB, vcp.rB) - vA - MathUtils.Cross(wA, vcp.rA);

                    // Compute normal impulse
                    FP vn = TSVector2.Dot(dv, normal);
                    FP lambda = -vcp.normalMass * (vn - vcp.velocityBias);

                    // b2Clamp the accumulated impulse
                    FP newImpulse = TrueSync.TSMath.Max(vcp.normalImpulse + lambda, 0.0f);
                    lambda = newImpulse - vcp.normalImpulse;
                    vcp.normalImpulse = newImpulse;

                    // Apply contact impulse
                    TSVector2 P = lambda * normal;
                    vA -= mA * P;
                    wA -= iA * MathUtils.Cross(vcp.rA, P);

                    vB += mB * P;
                    wB += iB * MathUtils.Cross(vcp.rB, P);
                }
                else
                {
                    // Block solver developed in collaboration with Dirk Gregorius (back in 01/07 on Box2D_Lite).
                    // Build the mini LCP for this contact patch
                    //
                    // vn = A * x + b, vn >= 0, , vn >= 0, x >= 0 and vn_i * x_i = 0 with i = 1..2
                    //
                    // A = J * W * JT and J = ( -n, -r1 x n, n, r2 x n )
                    // b = vn0 - velocityBias
                    //
                    // The system is solved using the "Total enumeration method" (s. Murty). The complementary constraint vn_i * x_i
                    // implies that we must have in any solution either vn_i = 0 or x_i = 0. So for the 2D contact problem the cases
                    // vn1 = 0 and vn2 = 0, x1 = 0 and x2 = 0, x1 = 0 and vn2 = 0, x2 = 0 and vn1 = 0 need to be tested. The first valid
                    // solution that satisfies the problem is chosen.
                    // 
                    // In order to account of the accumulated impulse 'a' (because of the iterative nature of the solver which only requires
                    // that the accumulated impulse is clamped and not the incremental impulse) we change the impulse variable (x_i).
                    //
                    // Substitute:
                    // 
                    // x = a + d
                    // 
                    // a := old total impulse
                    // x := new total impulse
                    // d := incremental impulse 
                    //
                    // For the current iteration we extend the formula for the incremental impulse
                    // to compute the new total impulse:
                    //
                    // vn = A * d + b
                    //    = A * (x - a) + b
                    //    = A * x + b - A * a
                    //    = A * x + b'
                    // b' = b - A * a;

                    VelocityConstraintPoint cp1 = vc.points[0];
                    VelocityConstraintPoint cp2 = vc.points[1];

                    TSVector2 a = new TSVector2(cp1.normalImpulse, cp2.normalImpulse);
                    Debug.Assert(a.x >= 0.0f && a.y >= 0.0f);

                    // Relative velocity at contact
                    TSVector2 dv1 = vB + MathUtils.Cross(wB, cp1.rB) - vA - MathUtils.Cross(wA, cp1.rA);
                    TSVector2 dv2 = vB + MathUtils.Cross(wB, cp2.rB) - vA - MathUtils.Cross(wA, cp2.rA);

                    // Compute normal velocity
                    FP vn1 = TSVector2.Dot(dv1, normal);
                    FP vn2 = TSVector2.Dot(dv2, normal);

                    TSVector2 b = new TSVector2();
                    b.x = vn1 - cp1.velocityBias;
                    b.y = vn2 - cp2.velocityBias;

                    // Compute b'
                    b -= MathUtils.Mul(ref vc.K, a);

                    //FP k_errorTol = 1e-3f;
                    //B2_NOT_USED(k_errorTol);

                    for (; ; )
                    {
                        //
                        // Case 1: vn = 0
                        //
                        // 0 = A * x + b'
                        //
                        // Solve for x:
                        //
                        // x = - inv(A) * b'
                        //
                        TSVector2 x = -MathUtils.Mul(ref vc.normalMass, b);

                        if (x.x >= 0.0f && x.y >= 0.0f)
                        {
                            // Get the incremental impulse
                            TSVector2 d = x - a;

                            // Apply incremental impulse
                            TSVector2 P1 = d.x * normal;
                            TSVector2 P2 = d.y * normal;
                            vA -= mA * (P1 + P2);
                            wA -= iA * (MathUtils.Cross(cp1.rA, P1) + MathUtils.Cross(cp2.rA, P2));

                            vB += mB * (P1 + P2);
                            wB += iB * (MathUtils.Cross(cp1.rB, P1) + MathUtils.Cross(cp2.rB, P2));

                            // Accumulate
                            cp1.normalImpulse = x.x;
                            cp2.normalImpulse = x.y;

#if B2_DEBUG_SOLVER 
					// Postconditions
					dv1 = vB + MathUtils.Cross(wB, cp1.rB) - vA - MathUtils.Cross(wA, cp1.rA);
					dv2 = vB + MathUtils.Cross(wB, cp2.rB) - vA - MathUtils.Cross(wA, cp2.rA);

					// Compute normal velocity
					vn1 = Vector2.Dot(dv1, normal);
					vn2 = Vector2.Dot(dv2, normal);

					b2Assert(b2Abs(vn1 - cp1.velocityBias) < k_errorTol);
					b2Assert(b2Abs(vn2 - cp2.velocityBias) < k_errorTol);
#endif
                            break;
                        }

                        //
                        // Case 2: vn1 = 0 and x2 = 0
                        //
                        //   0 = a11 * x1 + a12 * 0 + b1' 
                        // vn2 = a21 * x1 + a22 * 0 + b2'
                        //
                        x.x = -cp1.normalMass * b.x;
                        x.y = 0.0f;
                        vn1 = 0.0f;
                        vn2 = vc.K.ex.y * x.x + b.y;

                        if (x.x >= 0.0f && vn2 >= 0.0f)
                        {
                            // Get the incremental impulse
                            TSVector2 d = x - a;

                            // Apply incremental impulse
                            TSVector2 P1 = d.x * normal;
                            TSVector2 P2 = d.y * normal;
                            vA -= mA * (P1 + P2);
                            wA -= iA * (MathUtils.Cross(cp1.rA, P1) + MathUtils.Cross(cp2.rA, P2));

                            vB += mB * (P1 + P2);
                            wB += iB * (MathUtils.Cross(cp1.rB, P1) + MathUtils.Cross(cp2.rB, P2));

                            // Accumulate
                            cp1.normalImpulse = x.x;
                            cp2.normalImpulse = x.y;

#if B2_DEBUG_SOLVER
					// Postconditions
					dv1 = vB + MathUtils.Cross(wB, cp1.rB) - vA - MathUtils.Cross(wA, cp1.rA);

					// Compute normal velocity
					vn1 = Vector2.Dot(dv1, normal);

					b2Assert(b2Abs(vn1 - cp1.velocityBias) < k_errorTol);
#endif
                            break;
                        }


                        //
                        // Case 3: vn2 = 0 and x1 = 0
                        //
                        // vn1 = a11 * 0 + a12 * x2 + b1' 
                        //   0 = a21 * 0 + a22 * x2 + b2'
                        //
                        x.x = 0.0f;
                        x.y = -cp2.normalMass * b.y;
                        vn1 = vc.K.ey.x * x.y + b.x;
                        vn2 = 0.0f;

                        if (x.y >= 0.0f && vn1 >= 0.0f)
                        {
                            // Resubstitute for the incremental impulse
                            TSVector2 d = x - a;

                            // Apply incremental impulse
                            TSVector2 P1 = d.x * normal;
                            TSVector2 P2 = d.y * normal;
                            vA -= mA * (P1 + P2);
                            wA -= iA * (MathUtils.Cross(cp1.rA, P1) + MathUtils.Cross(cp2.rA, P2));

                            vB += mB * (P1 + P2);
                            wB += iB * (MathUtils.Cross(cp1.rB, P1) + MathUtils.Cross(cp2.rB, P2));

                            // Accumulate
                            cp1.normalImpulse = x.x;
                            cp2.normalImpulse = x.y;

#if B2_DEBUG_SOLVER
					// Postconditions
					dv2 = vB + MathUtils.Cross(wB, cp2.rB) - vA - MathUtils.Cross(wA, cp2.rA);

					// Compute normal velocity
					vn2 = Vector2.Dot(dv2, normal);

					b2Assert(b2Abs(vn2 - cp2.velocityBias) < k_errorTol);
#endif
                            break;
                        }

                        //
                        // Case 4: x1 = 0 and x2 = 0
                        // 
                        // vn1 = b1
                        // vn2 = b2;
                        x.x = 0.0f;
                        x.y = 0.0f;
                        vn1 = b.x;
                        vn2 = b.y;

                        if (vn1 >= 0.0f && vn2 >= 0.0f)
                        {
                            // Resubstitute for the incremental impulse
                            TSVector2 d = x - a;

                            // Apply incremental impulse
                            TSVector2 P1 = d.x * normal;
                            TSVector2 P2 = d.y * normal;
                            vA -= mA * (P1 + P2);
                            wA -= iA * (MathUtils.Cross(cp1.rA, P1) + MathUtils.Cross(cp2.rA, P2));

                            vB += mB * (P1 + P2);
                            wB += iB * (MathUtils.Cross(cp1.rB, P1) + MathUtils.Cross(cp2.rB, P2));

                            // Accumulate
                            cp1.normalImpulse = x.x;
                            cp2.normalImpulse = x.y;

                            break;
                        }

                        // No solution, give up. This is hit sometimes, but it doesn't seem to matter.
                        break;
                    }
                }

                _velocities[indexA].v = vA;
                _velocities[indexA].w = wA;
                _velocities[indexB].v = vB;
                _velocities[indexB].w = wB;
            }
        }

        public void StoreImpulses()
        {
            for (int i = 0; i < _count; ++i)
            {
                ContactVelocityConstraint vc = _velocityConstraints[i];
                Manifold manifold = _contacts[vc.contactIndex].Manifold;

                for (int j = 0; j < vc.pointCount; ++j)
                {
                    ManifoldPoint point = manifold.Points[j];
                    point.NormalImpulse = vc.points[j].normalImpulse;
                    point.TangentImpulse = vc.points[j].tangentImpulse;
                    manifold.Points[j] = point;
                }

                _contacts[vc.contactIndex].Manifold = manifold;
            }
        }

        public bool SolvePositionConstraints()
        {
            FP minSeparation = 0.0f;

            for (int i = 0; i < _count; ++i)
            {
                ContactPositionConstraint pc = _positionConstraints[i];

                int indexA = pc.indexA;
                int indexB = pc.indexB;
                TSVector2 localCenterA = pc.localCenterA;
                FP mA = pc.invMassA;
                FP iA = pc.invIA;
                TSVector2 localCenterB = pc.localCenterB;
                FP mB = pc.invMassB;
                FP iB = pc.invIB;
                int pointCount = pc.pointCount;

                TSVector2 cA = _positions[indexA].c;
                FP aA = _positions[indexA].a;

                TSVector2 cB = _positions[indexB].c;
                FP aB = _positions[indexB].a;

                // Solve normal constraints
                for (int j = 0; j < pointCount; ++j)
                {
                    Transform xfA = new Transform();
                    Transform xfB = new Transform();
                    xfA.q.Set(aA);
                    xfB.q.Set(aB);
                    xfA.p = cA - MathUtils.Mul(xfA.q, localCenterA);
                    xfB.p = cB - MathUtils.Mul(xfB.q, localCenterB);

                    TSVector2 normal;
                    TSVector2 point;
                    FP separation;

                    PositionSolverManifold.Initialize(pc, xfA, xfB, j, out normal, out point, out separation);

                    TSVector2 rA = point - cA;
                    TSVector2 rB = point - cB;

                    // Track max constraint error.
                    minSeparation = TrueSync.TSMath.Min(minSeparation, separation);

                    // Prevent large corrections and allow slop.
                    FP C = MathUtils.Clamp(Settings.Baumgarte * (separation + Settings.LinearSlop), -Settings.MaxLinearCorrection, 0.0f);

                    // Compute the effective mass.
                    FP rnA = MathUtils.Cross(rA, normal);
                    FP rnB = MathUtils.Cross(rB, normal);
                    FP K = mA + mB + iA * rnA * rnA + iB * rnB * rnB;

                    // Compute normal impulse
                    FP impulse = K > 0.0f ? -C / K : 0.0f;

                    TSVector2 P = impulse * normal;

                    cA -= mA * P;
                    aA -= iA * MathUtils.Cross(rA, P);

                    cB += mB * P;
                    aB += iB * MathUtils.Cross(rB, P);
                }

                _positions[indexA].c = cA;
                _positions[indexA].a = aA;

                _positions[indexB].c = cB;
                _positions[indexB].a = aB;
            }

            // We can't expect minSpeparation >= -b2_linearSlop because we don't
            // push the separation above -b2_linearSlop.
            return minSeparation >= -3.0f * Settings.LinearSlop;
        }

        // Sequential position solver for position constraints.
        public bool SolveTOIPositionConstraints(int toiIndexA, int toiIndexB)
        {
            FP minSeparation = 0.0f;

            for (int i = 0; i < _count; ++i)
            {
                ContactPositionConstraint pc = _positionConstraints[i];

                int indexA = pc.indexA;
                int indexB = pc.indexB;
                TSVector2 localCenterA = pc.localCenterA;
                TSVector2 localCenterB = pc.localCenterB;
                int pointCount = pc.pointCount;

                FP mA = 0.0f;
                FP iA = 0.0f;
                if (indexA == toiIndexA || indexA == toiIndexB)
                {
                    mA = pc.invMassA;
                    iA = pc.invIA;
                }

                FP mB = 0.0f;
                FP iB = 0.0f;
                if (indexB == toiIndexA || indexB == toiIndexB)
                {
                    mB = pc.invMassB;
                    iB = pc.invIB;
                }

                TSVector2 cA = _positions[indexA].c;
                FP aA = _positions[indexA].a;

                TSVector2 cB = _positions[indexB].c;
                FP aB = _positions[indexB].a;

                // Solve normal constraints
                for (int j = 0; j < pointCount; ++j)
                {
                    Transform xfA = new Transform();
                    Transform xfB = new Transform();
                    xfA.q.Set(aA);
                    xfB.q.Set(aB);
                    xfA.p = cA - MathUtils.Mul(xfA.q, localCenterA);
                    xfB.p = cB - MathUtils.Mul(xfB.q, localCenterB);

                    TSVector2 normal;
                    TSVector2 point;
                    FP separation;

                    PositionSolverManifold.Initialize(pc, xfA, xfB, j, out normal, out point, out separation);

                    TSVector2 rA = point - cA;
                    TSVector2 rB = point - cB;

                    // Track max constraint error.
                    minSeparation = TrueSync.TSMath.Min(minSeparation, separation);

                    // Prevent large corrections and allow slop.
                    FP C = MathUtils.Clamp(Settings.Baumgarte * (separation + Settings.LinearSlop), -Settings.MaxLinearCorrection, 0.0f);

                    // Compute the effective mass.
                    FP rnA = MathUtils.Cross(rA, normal);
                    FP rnB = MathUtils.Cross(rB, normal);
                    FP K = mA + mB + iA * rnA * rnA + iB * rnB * rnB;

                    // Compute normal impulse
                    FP impulse = K > 0.0f ? -C / K : 0.0f;

                    TSVector2 P = impulse * normal;

                    cA -= mA * P;
                    aA -= iA * MathUtils.Cross(rA, P);

                    cB += mB * P;
                    aB += iB * MathUtils.Cross(rB, P);
                }

                _positions[indexA].c = cA;
                _positions[indexA].a = aA;

                _positions[indexB].c = cB;
                _positions[indexB].a = aB;
            }

            // We can't expect minSpeparation >= -b2_linearSlop because we don't
            // push the separation above -b2_linearSlop.
            return minSeparation >= -1.5f * Settings.LinearSlop;
        }

        public static class WorldManifold
        {
            /// <summary>
            /// Evaluate the manifold with supplied transforms. This assumes
            /// modest motion from the original state. This does not change the
            /// point count, impulses, etc. The radii must come from the Shapes
            /// that generated the manifold.
            /// </summary>
            /// <param name="manifold">The manifold.</param>
            /// <param name="xfA">The transform for A.</param>
            /// <param name="radiusA">The radius for A.</param>
            /// <param name="xfB">The transform for B.</param>
            /// <param name="radiusB">The radius for B.</param>
            /// <param name="normal">World vector pointing from A to B</param>
            /// <param name="points">Torld contact point (point of intersection).</param>
            public static void Initialize(ref Manifold manifold, ref Transform xfA, FP radiusA, ref Transform xfB, FP radiusB, out TSVector2 normal, out FixedArray2<TSVector2> points)
            {
                normal = TSVector2.zero;
                points = new FixedArray2<TSVector2>();

                if (manifold.PointCount == 0)
                {
                    return;
                }

                switch (manifold.Type)
                {
                    case ManifoldType.Circles:
                        {
                            normal = new TSVector2(1.0f, 0.0f);
                            TSVector2 pointA = MathUtils.Mul(ref xfA, manifold.LocalPoint);
                            TSVector2 pointB = MathUtils.Mul(ref xfB, manifold.Points[0].LocalPoint);
                            if (TSVector2.DistanceSquared(pointA, pointB) > Settings.EpsilonSqr)
                            {
                                normal = pointB - pointA;
                                normal.Normalize();
                            }

                            TSVector2 cA = pointA + radiusA * normal;
                            TSVector2 cB = pointB - radiusB * normal;
                            points[0] = 0.5f * (cA + cB);
                        }
                        break;

                    case ManifoldType.FaceA:
                        {
                            normal = MathUtils.Mul(xfA.q, manifold.LocalNormal);
                            TSVector2 planePoint = MathUtils.Mul(ref xfA, manifold.LocalPoint);

                            for (int i = 0; i < manifold.PointCount; ++i)
                            {
                                TSVector2 clipPoint = MathUtils.Mul(ref xfB, manifold.Points[i].LocalPoint);
                                TSVector2 cA = clipPoint + (radiusA - TSVector2.Dot(clipPoint - planePoint, normal)) * normal;
                                TSVector2 cB = clipPoint - radiusB * normal;
                                points[i] = 0.5f * (cA + cB);
                            }
                        }
                        break;

                    case ManifoldType.FaceB:
                        {
                            normal = MathUtils.Mul(xfB.q, manifold.LocalNormal);
                            TSVector2 planePoint = MathUtils.Mul(ref xfB, manifold.LocalPoint);

                            for (int i = 0; i < manifold.PointCount; ++i)
                            {
                                TSVector2 clipPoint = MathUtils.Mul(ref xfA, manifold.Points[i].LocalPoint);
                                TSVector2 cB = clipPoint + (radiusB - TSVector2.Dot(clipPoint - planePoint, normal)) * normal;
                                TSVector2 cA = clipPoint - radiusA * normal;
                                points[i] = 0.5f * (cA + cB);
                            }

                            // Ensure normal points from A to B.
                            normal = -normal;
                        }
                        break;
                }
            }
        }

        private static class PositionSolverManifold
        {
            public static void Initialize(ContactPositionConstraint pc, Transform xfA, Transform xfB, int index, out TSVector2 normal, out TSVector2 point, out FP separation)
            {
                Debug.Assert(pc.pointCount > 0);


                switch (pc.type)
                {
                    case ManifoldType.Circles:
                        {
                            TSVector2 pointA = MathUtils.Mul(ref xfA, pc.localPoint);
                            TSVector2 pointB = MathUtils.Mul(ref xfB, pc.localPoints[0]);
                            normal = pointB - pointA;
                            normal.Normalize();
                            point = 0.5f * (pointA + pointB);
                            separation = TSVector2.Dot(pointB - pointA, normal) - pc.radiusA - pc.radiusB;
                        }
                        break;

                    case ManifoldType.FaceA:
                        {
                            normal = MathUtils.Mul(xfA.q, pc.localNormal);
                            TSVector2 planePoint = MathUtils.Mul(ref xfA, pc.localPoint);

                            TSVector2 clipPoint = MathUtils.Mul(ref xfB, pc.localPoints[index]);
                            separation = TSVector2.Dot(clipPoint - planePoint, normal) - pc.radiusA - pc.radiusB;
                            point = clipPoint;
                        }
                        break;

                    case ManifoldType.FaceB:
                        {
                            normal = MathUtils.Mul(xfB.q, pc.localNormal);
                            TSVector2 planePoint = MathUtils.Mul(ref xfB, pc.localPoint);

                            TSVector2 clipPoint = MathUtils.Mul(ref xfA, pc.localPoints[index]);
                            separation = TSVector2.Dot(clipPoint - planePoint, normal) - pc.radiusA - pc.radiusB;
                            point = clipPoint;

                            // Ensure normal points from A to B
                            normal = -normal;
                        }
                        break;
                    default:
                        normal = TSVector2.zero;
                        point = TSVector2.zero;
                        separation = 0;
                        break;

                }
            }
        }
    }
}