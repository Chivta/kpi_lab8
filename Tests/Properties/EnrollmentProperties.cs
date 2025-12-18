using FsCheck;
using FsCheck.Xunit;
using Online_Course_Enrollment_System_Properties.Domain;
using Online_Course_Enrollment_System_Properties.Tests.Arbitraries;
using System;
using Xunit;

namespace Online_Course_Enrollment_System_Properties.Tests.Properties
{
    public class EnrollmentProperties
    {
        [Property(Arbitrary = new[] { typeof(CourseArbitraries) })]
        public void StudentIdIsNeverNullOrWhitespace(Student student)
        {
            Assert.False(string.IsNullOrWhiteSpace(student.Id));
        }

        [Property(Arbitrary = new[] { typeof(CourseArbitraries) })]
        public void CourseIdIsNeverNullOrWhitespace(Course course)
        {
            Assert.False(string.IsNullOrWhiteSpace(course.Id));
        }

        [Property(Arbitrary = new[] { typeof(CourseArbitraries) })]
        public void CourseCapacityIsAlwaysPositive(Course course)
        {
            Assert.True(course.Capacity > 0);
        }

        [Property(Arbitrary = new[] { typeof(CourseArbitraries) })]
        public void EnrolledStudentsNeverExceedsCapacity(Course course)
        {
            Assert.True(course.Students.Count <= course.Capacity);
        }

        [Property(Arbitrary = new[] { typeof(CourseArbitraries) })]
        public void EnrollingSameStudentTwiceReturnsFalse(Course course, Student student)
        {
            var firstEnrollment = course.Enroll(student);
            var secondEnrollment = course.Enroll(student);

            Assert.False(secondEnrollment);
        }

        [Property(Arbitrary = new[] { typeof(CourseArbitraries) })]
        public void CannotEnrollWhenCourseFull(Course course)
        {
            // Fill the course to capacity
            for (int i = 0; i < course.Capacity; i++)
            {
                var student = new Student($"Student{i}");
                course.Enroll(student);
            }

            // Try to enroll one more
            var extraStudent = new Student("ExtraStudent");
            var result = course.Enroll(extraStudent);

            Assert.False(result);
            Assert.Equal(course.Capacity, course.Students.Count);
        }

        [Property(Arbitrary = new[] { typeof(CourseArbitraries) })]
        public void EnrollmentRepositorySavesAndRetrievesCourses(Course course1, Course course2)
        {
            var repo = new EnrollmentRepository();

            repo.Save(course1);
            repo.Save(course2);

            var allCourses = repo.All();

            Assert.Equal(2, allCourses.Count);
            Assert.Contains(course1, allCourses);
            Assert.Contains(course2, allCourses);
        }

        [Property(Arbitrary = new[] { typeof(CourseArbitraries) })]
        public void NewCourseStartsWithZeroStudents(Course course)
        {
            Assert.Empty(course.Students);
        }

        [Property(Arbitrary = new[] { typeof(CourseArbitraries) })]
        public void SuccessfulEnrollmentIncreasesStudentCount(Course course, Student student)
        {
            var initialCount = course.Students.Count;
            var enrolled = course.Enroll(student);

            if (enrolled)
            {
                Assert.Equal(initialCount + 1, course.Students.Count);
                Assert.Contains(student, course.Students);
            }
            else
            {
                Assert.Equal(initialCount, course.Students.Count);
            }
        }

        [Property(Arbitrary = new[] { typeof(CourseArbitraries) })]
        public void EnrollmentSucceedsWhenCapacityAvailable(Student student)
        {
            var course = new Course("TestCourse", 10);
            var result = course.Enroll(student);

            Assert.True(result);
            Assert.Single(course.Students);
            Assert.Contains(student, course.Students);
        }
    }

}
