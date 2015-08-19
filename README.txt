Design Document: Boids 2.0


Head Designer and Programer: Quinton Baudoin




Introduction:
                
                Uses Boid Flocking algorithm to get a group of objects to move with a simulated flocking behavior.


Descriptions:
                
                This project primarily consists of the use of Cohesion, Separation, and Alignment.
Cohesion:
                Cohesion is a boid move towards another boid in its flock or towards the flock of boids.        This is done by getting the perceived center of mass, The center of mass of all OTHER boids, and creating a velocity that is in its direction.        


Separation:


                Separation is when a boid corrects its path if it is too close to another boid.                This is done by first checking its distance with another boid and if it is within a set distance a velocity in the opposite direction is applied.


Alignment:
                
                Alignment is when a boid moves with a velocity simular to a boid or boids near itself.        This is done by getting the average velocity of all the boids that are within a certain radius of itself and applies it to its own velocity.




Other Implementations used in project:


        Cohesion is only affected by boids within a threshold of the boid as is alignment. Boids previous velocity reduced by half of its normal and applied to the new velocity to keep current direction but reduce its impact on the speed of the boid’s new velocity. spawning is on a timer of 1 a second, excluding the initial two which spawns without the timer delay.












Inputs:
 This SImulation is managed by 4 on screen sliders and 2 on screen buttons.


Sliders: Location: Lower Left corner: Order From top to bottom


Separation Distance: Color: Light Cyan


        Raises or lowers the distance boids will get before they decide to separate.Moving the slider left decreases the distance: Moving the slider right increases the distance.


NOTE: Adjusting this slider does not impact the speed they will go to separate.


Cohesion speed: Color: Red


        Raises or lowers the pull of boids towards its perceived center of mass. Moving the slider left decreases the pull: moving the slider right increases the pull


Separation Speed: Color: Yellow
        
        Raises or lower the push away from other boids. Moving the slider left decreases the push: moving the slider right increases the push.


NOTE: Adjusting this slider does not impact the distance the boids will decide to separate.


Velocity limit: Color: White


        Raises or lowers the maximum velocity a boid can go. Moving the slider left reduces the maximum velocity; moving the slider right increases the max velocity.


NOTE: This does not affect the ratio between the cohesion, separation, and alignment velocity, but instead checks the final velocity calculated.


Buttons: Location: Top of screen


        Follow/Unfollow Boid: Location: Left
        
                Changes the camera's position from fixed position to following a boid and vice versa. When Fallow is the visible text press the button to follow a random boid in a first-person perspective; when Unfollow Boid is the text visible press button to stop following the boid and return to a fixed position.


NOTE: This is only camara view. User is unable to actually control the boid.


Exit: Location: Right
                
                Closes application. Press the button to close the application.


NOTE: There is no saving; Any slider adjustments will be reverted back to its original position on reboot of application.