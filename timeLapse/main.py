import cv2
import os
import re

# Set the directory containing the images
use_shuffle = True
if use_shuffle:
    image_folder = 'C:/Users/murschac/Pictures/rayTracing/shuffle'
    image_regex = re.compile(r'image_\d{4}.bmp')
else:
    image_folder = 'C:/Users/murschac/Pictures/rayTracing'
    image_regex = re.compile(r'image_\d{4}.png')


image_files = [f for f in os.listdir(image_folder) if image_regex.match(f)]

# Sort the image files
image_files.sort()

# Read the first image to get dimensions
first_image_path = os.path.join(image_folder, image_files[0])
frame = cv2.imread(first_image_path)
height, width, layers = frame.shape

# Define the codec and create a VideoWriter object for iphone
output_path = os.path.join(image_folder, 'time_lapse_video.mp4')
fourcc = cv2.VideoWriter_fourcc(*'mp4v')

# the frame rate should ensure that the video length time is 5 seconds long given the length of the image_files array
video_length_seconds = 7
frame_rate = int(len(image_files) / video_length_seconds)

video = cv2.VideoWriter(output_path, fourcc, frame_rate, (width, height))

# Iterate over the images and write them to the video
for image in image_files:
    image_path = os.path.join(image_folder, image)
    frame = cv2.imread(image_path)
    video.write(frame)

# Make the last image stay for an additional x seconds
last_frame_seconds = 4
last_image_path = os.path.join(image_folder, image_files[-1])
last_image_path = os.path.join(image_folder, 'image.png')
last_frame = cv2.imread(last_image_path)
for _ in range(int(last_frame_seconds * frame_rate)):
    video.write(last_frame)

# Release the video writer object
video.release()
cv2.destroyAllWindows()


print('Video created successfully!')