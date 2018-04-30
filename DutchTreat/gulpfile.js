/// <binding AfterBuild='default' />
var gulp = require('gulp');
var uglify = require("gulp-uglify");
var concat = require("gulp-concat");
var gutil = require('gulp-util');

gulp.task("minify", function () {
  return gulp.src("wwwroot/js/**/*.js")
    .pipe(uglify().on('error', gutil.log))
    .pipe(concat("dutchtreat.min.js"))
    .pipe(gulp.dest("wwwroot/dist"));
});

gulp.task('default', ["minify"]);