# Generated by Django 5.0 on 2023-12-07 12:30

import django.utils.timezone
from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('aplikacja', '0002_usermanager'),
    ]

    operations = [
        migrations.AlterField(
            model_name='log',
            name='timestamp',
            field=models.DateTimeField(default=django.utils.timezone.now),
        ),
        migrations.AlterField(
            model_name='userloginhistory',
            name='login_time',
            field=models.DateTimeField(default=django.utils.timezone.now),
        ),
    ]
