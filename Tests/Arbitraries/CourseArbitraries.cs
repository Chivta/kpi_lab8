using FsCheck;
using FsCheck.FSharp;
using Microsoft.FSharp.Core;
using Online_Course_Enrollment_System_Properties.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Online_Course_Enrollment_System_Properties.Tests.Arbitraries
{
    public static class CourseArbitraries
    {
        public static Arbitrary<Student> Student()
        {
            // Generate strings with alphanumeric characters and spaces
            var chars = new[] {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_', ' '
            };
            var charGen = Gen.Elements(chars);

            var stringGen = GenBuilder.gen.Bind<int, Student>(
                Gen.Choose(1, 50),
                FuncConvert.FromFunc<int, Gen<Student>>(length =>
                    GenBuilder.gen.Bind<char[], Student>(
                        Gen.ArrayOf<char>(length, charGen),
                        FuncConvert.FromFunc<char[], Gen<Student>>(charArray =>
                        {
                            var str = new string(charArray).Trim();
                            if (string.IsNullOrWhiteSpace(str))
                                return GenBuilder.gen.Return(new Student("A")); // Fallback
                            return GenBuilder.gen.Return(new Student(str));
                        }))));

            return Arb.From(stringGen);
        }

        public static Arbitrary<Course> Course()
        {
            var chars = new[] {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_', ' '
            };
            var charGen = Gen.Elements(chars);

            var gen = GenBuilder.gen.Bind<int, Domain.Course>(
                Gen.Choose(1, 50),
                FuncConvert.FromFunc<int, Gen<Domain.Course>>(idLength =>
                    GenBuilder.gen.Bind<char[], Domain.Course>(
                        Gen.ArrayOf<char>(idLength, charGen),
                        FuncConvert.FromFunc<char[], Gen<Domain.Course>>(charArray =>
                        {
                            var id = new string(charArray).Trim();
                            if (string.IsNullOrWhiteSpace(id))
                                id = "Course1"; // Fallback

                            return GenBuilder.gen.Bind<int, Domain.Course>(
                                Gen.Choose(1, 1000),
                                FuncConvert.FromFunc<int, Gen<Domain.Course>>(capacity =>
                                    GenBuilder.gen.Return(new Domain.Course(id, capacity))));
                        }))));

            return Arb.From(gen);
        }

        public static Arbitrary<Course> CourseWithStudents()
        {
            var gen = GenBuilder.gen.Bind<Domain.Course, Domain.Course>(
                Course().Generator,
                FuncConvert.FromFunc<Domain.Course, Gen<Domain.Course>>(course =>
                    GenBuilder.gen.Bind<int, Domain.Course>(
                        Gen.Choose(0, course.Capacity),
                        FuncConvert.FromFunc<int, Gen<Domain.Course>>(studentCount =>
                            GenBuilder.gen.Bind<Microsoft.FSharp.Collections.FSharpList<Student>, Domain.Course>(
                                Gen.ListOf(studentCount, Student().Generator),
                                FuncConvert.FromFunc<Microsoft.FSharp.Collections.FSharpList<Student>, Gen<Domain.Course>>(fsharpList =>
                                {
                                    var students = Microsoft.FSharp.Collections.ListModule.ToArray(fsharpList).ToList();
                                    return GenBuilder.gen.Return(EnrollStudentsInCourse(course, students));
                                }))))));

            return Arb.From(gen);
        }

        private static Course EnrollStudentsInCourse(Course course, List<Student> students)
        {
            var uniqueStudents = students.Distinct().Take(course.Capacity).ToList();
            foreach (var student in uniqueStudents)
            {
                course.Enroll(student);
            }
            return course;
        }
    }
}
