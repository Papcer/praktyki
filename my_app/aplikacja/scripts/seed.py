import random

from argon2 import PasswordHasher
from django_seed import Seed
from django.contrib.auth.hashers import make_password
from aplikacja.models import User, Role, UserRoles
import secrets


def set_password(self, raw_password):
    ph = PasswordHasher()
    self.password = ph.hash(raw_password)
def seed_database():
    seeder = Seed.seeder()
    
    # Roles
    roles = ["admin", "office_employee", "customer_service_employee", "marketing_team_employee", "it_team_employee"]
    
    for role in roles:
        Role.objects.get_or_create(role_name=role)
    
    # Users
    for role in roles:
        user_data = {
            "username": f"{role}@epicup.pl",
            "password": set_password('test'),
        }
        user = User(**user_data)
        user.save()
    
        role_object = Role.objects.get(role_name=role)
        user_roles = UserRoles(user=user, role=role_object)
        user_roles.save()
    
    print("Database seeded successfully.")