from argon2.exceptions import VerifyMismatchError
from django.db import models
from django.contrib.auth.models import AbstractBaseUser, BaseUserManager
from argon2 import PasswordHasher
from django.utils import timezone

class User(AbstractBaseUser):
    username = models.CharField(max_length=150, unique=True)
    password = models.CharField(max_length=150)
    
    USERNAME_FIELD = 'username'
    REQUIRED_FIELDS = []
    
    def __str__(self):
        return self.username

    def save(self, *args, **kwargs):
        super().save(*args, **kwargs)

    def set_password(self, raw_password):
        ph = PasswordHasher()
        self.password = ph.hash(raw_password)

    def check_password(self, raw_password):
        ph = PasswordHasher()
        try:
            ph.verify(self.password, raw_password)
            return True
        except VerifyMismatchError:
            return False

    @classmethod
    def create_user(cls, username, password=None):
        user = cls(username=username)
        user.set_password(password)
        user.save()
        return user

    
class Role(models.Model):
    role_name = models.CharField(max_length=255, unique=True)

class UserRoles(models.Model):
    user = models.ForeignKey(User, on_delete=models.CASCADE)
    role = models.ForeignKey(Role, on_delete=models.CASCADE)

class Event(models.Model):
    title = models.CharField(max_length=255)
    start_date = models.DateTimeField()
    location = models.CharField(max_length=255)
    ticket_limit = models.PositiveIntegerField(default=100)

class UserData(models.Model):
    user = models.OneToOneField(User, on_delete=models.CASCADE)
    firstName = models.CharField(max_length=255)
    lastName = models.CharField(max_length=255)
    phoneNumber = models.CharField(max_length=11)
    #city = models.CharField(max_length=255)
    #zipcode = models.CharField(max_length=10)
    #street = models.CharField(max_length=255)
    
class Ticket(models.Model):
    event = models.ForeignKey(Event, on_delete=models.CASCADE)
    userdata = models.ForeignKey(UserData, on_delete=models.CASCADE, null=True, blank=True)


